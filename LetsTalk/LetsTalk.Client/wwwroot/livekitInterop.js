let room = null;

window.livekitInterop = {
    join: async (url, token) => {
        room = new LivekitClient.Room();
        await room.connect(url, token);

        await room.localParticipant.setMicrophoneEnabled(true);

        room.on("trackSubscribed", (track) => {
            if (track.kind === "audio") {
                // .attach() sans arguments crée un élément <audio> automatiquement
                const audioElement = track.attach();

                // On l'ajoute au body
                document.body.appendChild(audioElement);

                // On force la lecture (pour gérer les restrictions navigateurs)
                // 3. Appeler play manuellement avec une sécurité
                if (typeof audioElement.play === "function") {
                    audioElement.play().catch(err => {
                        console.warn("Autoplay bloqué ou erreur de lecture:", err);
                    });
                } else {
                    console.error("L'élément créé n'est pas un HTMLMediaElement valide", audioElement);
                }
            }
        });

        console.log("LiveKit connecté");
    },

    leave: () => {
        if (room) {
            room.disconnect();
            room = null;
        }
    }
};
