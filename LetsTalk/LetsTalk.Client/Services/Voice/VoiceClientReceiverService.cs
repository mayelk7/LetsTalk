using Concentus;
using Concentus.Structs;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsTalk.Client.Services.Voice
{
    public class VoiceClientReceiverService
    {
        private readonly WaveOutEvent _output;
        private readonly BufferedWaveProvider _buffer;
        private readonly OpusDecoder _decoder;

        // CancellationTokenSource pour gérer l'arrêt propre de la boucle de réception
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private const int SampleRate = 48000;
        private const int Channels = 1;

        public VoiceClientReceiverService()
        {
            // Initialisation audio
            _decoder = new OpusDecoder(SampleRate, Channels);
            _buffer = new BufferedWaveProvider(new WaveFormat(SampleRate, 16, Channels));
            _buffer.DiscardOnBufferOverflow = true; // Éviter l'accumulation en cas de latence

            _output = new WaveOutEvent();
            _output.Init(_buffer);
            _output.Play();
        }

        /// <summary>
        /// Démarre la boucle de réception audio. 
        /// Nécessite l'instance UdpClient partagée avec le service d'envoi.
        /// </summary>
        public async Task StartListeningAsync(UdpClient sharedUdp)
        {
            // 1. Utiliser le client UDP partagé
            UdpClient udp = sharedUdp;
            CancellationToken token = _cts.Token;

            // 2. Boucle de réception asynchrone pour ne pas bloquer le thread appelant
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Utilisation de ReceiveAsync pour ne pas bloquer
                    UdpReceiveResult result = await udp.ReceiveAsync(token);
                    byte[] fullData = result.Buffer;

                    // 3. Traiter le paquet reçu pour séparer l'en-tête des données binaires Opus
                    ProcessReceivedPacket(fullData);
                }
                catch (OperationCanceledException)
                {
                    // L'annulation est propre (appelé par Stop())
                    break;
                }
                catch (SocketException ex) when (ex.ErrorCode == 10004) // L'erreur 10004 est souvent liée à la fermeture forcée du socket
                {
                    // Le socket a été fermé par Stop(), arrêter proprement.
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur de réception : {ex.Message}");
                    // Continuer la boucle après une erreur
                }
            }
        }

        /// <summary>
        /// Extrait les données Opus, les décode et les ajoute au buffer de lecture.
        /// </summary>
        private void ProcessReceivedPacket(byte[] fullData)
        {
            // Trouver la fin de l'en-tête (ex: après "VOICE|Pseudo|Canal|")
            string headerEnd = "|";
            string headerAttempt = Encoding.UTF8.GetString(fullData[..Math.Min(fullData.Length, 256)]);
            int headerEndIndex = headerAttempt.IndexOf(headerEnd, StringComparison.Ordinal);

            if (headerEndIndex > 0)
            {
                string header = headerAttempt[..(headerEndIndex + headerEnd.Length)];
                string[] parts = header.Split('|');

                if (parts.Length >= 3 && parts[0] == "VOICE")
                {
                    // Les données Opus brutes commencent juste après l'en-tête
                    int opusDataStartIndex = Encoding.UTF8.GetBytes(header).Length;

                    // Séparation des données Opus
                    byte[] opusData = new byte[fullData.Length - opusDataStartIndex];
                    Buffer.BlockCopy(fullData, opusDataStartIndex, opusData, 0, opusData.Length);

                    // --- Décodage Opus ---
                    short[] pcm = new short[4096];
                    int samples = _decoder.Decode(opusData, 0, opusData.Length, pcm, 0, pcm.Length);

                    // Conversion de short[] (PCM) à byte[] (pour NAudio)
                    byte[] pcmBytes = new byte[samples * 2];
                    Buffer.BlockCopy(pcm, 0, pcmBytes, 0, pcmBytes.Length);

                    // Ajout au buffer pour la lecture
                    _buffer.AddSamples(pcmBytes, 0, pcmBytes.Length);
                }
                // Optionnel : Gérer les messages de contrôle ici (JOIN, LEAVE, PING, etc.)
                else if (parts.Length >= 2 && (parts[0] == "JOIN" || parts[0] == "LEAVE"))
                {
                    // Traiter les notifications d'entrée/sortie de canal ici
                    System.Diagnostics.Debug.WriteLine($"Notification: {parts[0]} {parts[1]}");
                }
            }
        }


        /// <summary>
        /// Arrête l'audio et la boucle de réception.
        /// </summary>
        public void StopListening()
        {
            // 1. Annuler la boucle de réception proprement
            _cts.Cancel();

            // 2. Arrêter l'audio et libérer les ressources
            _output.Stop();
            _output.Dispose();
            _cts.Dispose();
        }
    }
}