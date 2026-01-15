let room = null;
let currentUserId = null;

window.livekitInterop = {
    // ✅ Join avec l'userId en paramètre
    join: async (url, token, userId) => {
        currentUserId = userId;
        room = new LivekitClient.Room();

        await room.connect(url, token);
        console.log(`✅ Connecté en tant que ${userId}`);

        // Activer le micro
        await room.localParticipant.setMicrophoneEnabled(true);

        // Participants déjà présents
        room.remoteParticipants.forEach((participant) => {
            subscribeToParticipant(participant);
        });

        // Nouvelles pistes
        room.on("trackSubscribed", (track, publication, participant) => {
            console.log("🔔 TRACK REÇU:");
            console.log("  Participant:", participant.identity);
            console.log("  Kind:", track.kind); // "audio" ou "video"
            console.log("  Source:", track.source); // "microphone", "screen_share", etc.
            console.log("  Track complet:", track);
            handleTrack(track, participant);
        });

        // Nouveau participant
        room.on("participantConnected", (participant) => {
            console.log(`👋 ${participant.identity} a rejoint`);
            subscribeToParticipant(participant);
        });

        // Participant parti
        room.on("participantDisconnected", (participant) => {
            console.log(`👋 ${participant.identity} est parti`);
            removeParticipantAudio(participant);
            updateParticipantSpeaking(participant.identity, false);
        });

        // ✅ DÉTECTION DE QUI PARLE
        room.on("activeSpeakersChanged", (speakers) => {
            // Retirer tous les styles "speaking"
            document.querySelectorAll('.participant-card.speaking').forEach(card => {
                card.classList.remove('speaking');
            });

            // Ajouter le style pour ceux qui parlent
            speakers.forEach(speaker => {
                updateParticipantSpeaking(speaker.identity, true);
                console.log(`🗣️ ${speaker.identity} parle`);
            });
        });
    },

    toggleMute: async (muted) => {
        if (room && room.localParticipant) {
            await room.localParticipant.setMicrophoneEnabled(!muted);
            console.log(muted ? "🔇 Micro coupé" : "🎤 Micro activé");
        }
    },

    clearAllSpeaking: () => {
        document.querySelectorAll('.participant-card-wrapper.speaking').forEach(wrapper => {
            wrapper.classList.remove('speaking');
        });
        console.log("🧹 Tous les styles speaking retirés");
    },

    // ✅ NOUVELLE FONCTION : Partage d'écran
    toggleScreenShare: async (enable) => {
        if (!room) return;

        try {
            await room.localParticipant.setScreenShareEnabled(enable);
            console.log(enable
                ? "✅ Screen share activé"
                : "🛑 Screen share désactivé");
        }
        catch (error) {
            console.error("❌ Erreur screen share:", error);
        }
    },


    leave: () => {
        if (room) {
            // ✅ Arrêter le partage d'écran si actif
            if (screenShareTrack) {
                screenShareTrack[0].stop();
                screenShareTrack = null;
            }
            // Nettoyer les styles
            document.querySelectorAll('.participant-card.speaking').forEach(card => {
                card.classList.remove('speaking');
            });

            document.querySelectorAll('audio[data-livekit]').forEach(el => el.remove()); // Nettoyer les audio
            document.querySelectorAll('video[data-livekit]').forEach(el => el.remove()); // Nettoyer les vidéos

            room.disconnect();
            room = null;
            currentUserId = null;
            console.log("❌ Déconnecté");
        }
    },

    // ✅ Fonction pour synchroniser avec la liste Blazor
    updateParticipantsList: (participants) => {
        console.log("📋 Liste mise à jour:", participants);
    }
};

// ✅ Mettre à jour le style de la carte du participant qui parle
function updateParticipantSpeaking(identity, isSpeaking) {
    const wrapper = document.getElementById(`participant-card-${identity}`);
    console.log(`Mise à jour de ${identity} : speaking = ${isSpeaking}`);
    if (wrapper) {
        console.log("j'ai le wrapper");
        if (isSpeaking) {
            wrapper.classList.add('speaking');
            console.log("ajoute de speaking");
        } else {
            wrapper.classList.remove('speaking');
            console.log("retirer de speaking");
        }
    }
    console.log("j'ai le pas le wrapper"); 

}

function subscribeToParticipant(participant) {
    participant.audioTrackPublications.forEach((publication) => {
        if (publication.track) {
            handleTrack(publication.track, participant);
        }
    });
}

function handleTrack(track, participant) {
    console.log(track)
    if (track.kind === "audio") {
        console.log(`🎧 Audio de ${participant.identity}`);

        const audioElement = track.attach();
        audioElement.setAttribute('data-livekit', 'true');
        audioElement.setAttribute('data-participant', participant.identity);
        document.body.appendChild(audioElement);

        audioElement.play()
            .catch(err => console.warn(`Autoplay bloqué pour ${participant.identity}:`, err));
    }
    // ✅ NOUVEAU : Gérer la vidéo (partage d'écran)
    if (track.kind === "video") {
        console.log(`🖥️ Partage d'écran de ${participant.identity}`);
        const preview = document.getElementById(`screen-preview-${participant.identity}`);
        if (preview) {
            const videoElement = track.attach();
            videoElement.style.cssText = `
        width: 100%;
        height: 100%;
        object-fit: cover;
        border-radius: 4px;
    `;
            preview.appendChild(videoElement);
            preview.style.display = 'block';
        }
    } else {
        console.log("pas un cannaux video")
    }

    // Quand la piste se termine
    track.on('ended', () => {
        console.log(`🛑 Partage d'écran de ${participant.identity} terminé`);
        videoContainer.style.display = 'none';
    });
    
}

function removeParticipantAudio(participant) {
    const audioElements = document.querySelectorAll(`audio[data-participant="${participant.identity}"]`);
    audioElements.forEach(el => el.remove());
}