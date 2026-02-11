using Concentus;
using Concentus.Enums;
using Concentus.Structs;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics; // Ajout pour le débogage

namespace LetsTalk.Client.Services.Voice
{
    public class VoiceClientService
    {
        private WaveInEvent _waveIn;
        private readonly UdpClient _udp;
        private readonly OpusEncoder _encoder;
        private bool _disconnected = false;
        private readonly string _pseudo;
        private readonly string _canal;

        // Instance de la classe de réception
        private VoiceClientReceiverService? _receiverService;
        public VoiceClientReceiverService? ReceiverService { get; private set; }

        public VoiceClientService(string serverIp, string pseudo, string canal)
        {
            // Initialisation des propriétés du client
            _pseudo = pseudo;
            _canal = canal;

            // 1. Initialisation du client UDP et connexion
            _udp = new UdpClient();
            _udp.Connect(serverIp, 9000);

            // 2. Initialisation de l'encodeur Opus
            // Fréquence d'échantillonnage de 48 kHz (standard Opus), 1 canal (mono), application VoIP
            _encoder = new OpusEncoder(48000, 1, OpusApplication.OPUS_APPLICATION_VOIP);

            // 3. Initialisation de la capture audio (Micro)
            _waveIn = new WaveInEvent();
            _waveIn.WaveFormat = new WaveFormat(48000, 16, 1); // 48kHz, 16 bits, Mono
            _waveIn.BufferMilliseconds = 20; // 20ms de latence par paquet

            // Attribution de l'événement DataAvailable (Capture du microphone)
            _waveIn.DataAvailable += OnAudioDataAvailable;
        }

        /// <summary>
        /// Gère les données capturées par le microphone (méthode d'envoi).
        /// </summary>
        private void OnAudioDataAvailable(object? s, WaveInEventArgs e)
        {
            if (_disconnected) return;

            try
            {
                // Conversion du buffer byte[] (16 bits) en short[] (PCM)
                int samples = e.BytesRecorded / 2;
                short[] pcm = new short[samples];
                Buffer.BlockCopy(e.Buffer, 0, pcm, 0, e.BytesRecorded);

                // Encodage Opus
                byte[] encoded = new byte[4000]; // Buffer de sortie (taille max raisonnable)
                int encodedLength = _encoder.Encode(pcm, 0, samples, encoded, 0, encoded.Length);

                // Préfixe de l'en-tête pour le serveur (VOICE|pseudo|canal|)
                // L'en-tête doit être en UTF-8, suivi des données binaires (Opus)
                string header = $"VOICE|{_pseudo}|{_canal}|";
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                // Création du paquet final (En-tête + Données Opus)
                byte[] fullPacket = new byte[headerBytes.Length + encodedLength];
                Buffer.BlockCopy(headerBytes, 0, fullPacket, 0, headerBytes.Length);
                Buffer.BlockCopy(encoded, 0, fullPacket, headerBytes.Length, encodedLength);

                // Envoi du paquet complet
                _udp.Send(fullPacket, fullPacket.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur d'envoi de données vocales : {ex.Message}");
                // Gérer la déconnexion si l'erreur est grave (ex: SocketException)
                if (ex is SocketException)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Tente de se connecter au serveur et démarre l'enregistrement/la réception.
        /// </summary>
        public async Task StartAsync() // Changé en async Task
        {
            if (_disconnected)
            {
                // Le client est déjà déconnecté, ne pas tenter de démarrer
                return;
            }

            try
            {
                // 1. Envoyer paquet de connexion
                string authentificationVoice = $"CONNECT|{_pseudo}|{_canal}";
                byte[] authBytes = Encoding.UTF8.GetBytes(authentificationVoice);
                await _udp.SendAsync(authBytes, authBytes.Length); // Utilisation de SendAsync

                // 2. Attendre réponse du serveur (asynchrone)
                UdpReceiveResult result = await _udp.ReceiveAsync();
                string response = Encoding.UTF8.GetString(result.Buffer);

                if (response.StartsWith("ACCEPTED")) // Le serveur envoie peut-être ACCEPTED|pseudo
                {
                    // 3. Connexion acceptée → démarrer le micro
                    _waveIn.StartRecording();

                    ReceiverService = new VoiceClientReceiverService(); 
                                                                        
                    Task.Run(() => ReceiverService!.StartListeningAsync(_udp));

                    Debug.WriteLine("Client vocal connecté et prêt.");
                }
                else
                {
                    Debug.WriteLine($"Connexion refusée par le serveur : {response}");
                    _disconnected = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur de connexion : {ex.Message}");
                _disconnected = true;
                Stop(); // Nettoyage en cas d'échec de connexion
            }
        }

        /// <summary>
        /// Arrête l'enregistrement, la réception et ferme la connexion.
        /// </summary>
        public void Stop()
        {
            if (!_disconnected)
            {
                _disconnected = true;

                // 1. Envoyer un message de déconnexion propre (pour la gestion côté serveur)
                try
                {
                    string disconnectMessage = $"DISCONNECT|{_pseudo}|{_canal}";
                    byte[] disconnectBytes = Encoding.UTF8.GetBytes(disconnectMessage);
                    _udp.Send(disconnectBytes, disconnectBytes.Length);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de l'envoi du DISCONNECT: {ex.Message}");
                }

                // 2. Arrêter l'enregistrement
                _waveIn?.StopRecording();
                _waveIn?.Dispose();

                // 3. Arrêter le service de réception
                _receiverService?.StopListening();

                // 4. Fermer le socket
                _udp.Close();
                Debug.WriteLine("Client vocal déconnecté et nettoyé.");
            }
        }
    }
}