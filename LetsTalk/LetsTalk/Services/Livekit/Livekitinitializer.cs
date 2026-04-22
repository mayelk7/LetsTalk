using LetsTalk.Services.Livekit;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LetsTalk.Services.Livekit
{
    public class LivekitInitializer : IHostedService
    {
        private readonly LivekitService _livekitService;

        // On injecte le LivekitService ici
        public LivekitInitializer(LivekitService livekitService)
        {
            _livekitService = livekitService;
        }

        // Exécuté au démarrage de l'application
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Démarrage de l'initialisation LiveKit...");

            // ⚠️ ATTENTION : Vous devez déplacer la logique d'initialisation dans une méthode publique et l'attendre.
            await _livekitService.CreerSalonsInitiauxAsync();

            Console.WriteLine("Initialisation LiveKit terminée.");
        }

        // Exécuté à l'arrêt du service (souvent vide pour l'initialisation)
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}