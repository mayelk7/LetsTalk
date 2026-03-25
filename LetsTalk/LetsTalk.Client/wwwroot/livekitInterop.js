let room = null;
let currentUserId = null;

window.livekitInterop = {

    join: async (url, token, userId) => {
        currentUserId = userId;
        room = new LivekitClient.Room();

        await room.connect(url, token);
        console.log(`✅ Connecté en tant que ${userId}`);

        await room.localParticipant.setMicrophoneEnabled(true);

        // Participants déjà présents
        room.remoteParticipants.forEach((participant) => {
            subscribeToParticipant(participant);
        });

        // Nouvelles pistes (remote uniquement)
        room.on("trackSubscribed", (track, publication, participant) => {
            console.log("🔔 TRACK REÇU:");
            console.log("  Participant:", participant.identity);
            console.log("  Kind:", track.kind);
            console.log("  Source:", track.source);
            handleTrack(track, participant);
        });

        // ✅ Piste locale publiée (pour voir son propre screen share)
        room.localParticipant.on("localTrackPublished", (publication) => {
            const track = publication.track;
            if (track && track.kind === "video") {
                console.log("🖥️ Mon propre screen share publié");
                handleVideoTrack(track, room.localParticipant);
            }
        });

        // ✅ Piste locale dépubliée (on arrête le screen share)
        room.localParticipant.on("localTrackUnpublished", (publication) => {
            if (publication.kind === "video") {
                console.log("🛑 Mon screen share arrêté");
                removeVideoTrack(room.localParticipant.identity);
            }
        });

        room.on("participantConnected", (participant) => {
            console.log(`👋 ${participant.identity} a rejoint`);
            subscribeToParticipant(participant);
        });

        room.on("participantDisconnected", (participant) => {
            console.log(`👋 ${participant.identity} est parti`);
            removeParticipantAudio(participant);
            removeVideoTrack(participant.identity);
            updateParticipantSpeaking(participant.identity, false);
        });

        // ✅ Quand un remote arrête son screen share
        room.on("trackUnsubscribed", (track, publication, participant) => {
            if (track.kind === "video") {
                console.log(`🛑 Screen share de ${participant.identity} terminé`);
                removeVideoTrack(participant.identity);
            }
        });

        room.on("activeSpeakersChanged", (speakers) => {
            document.querySelectorAll('.participant-card.speaking').forEach(card => {
                card.classList.remove('speaking');
            });
            speakers.forEach(speaker => {
                updateParticipantSpeaking(speaker.identity, true);
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
    },

    // ✅ Screen share corrigé
    toggleScreenShare: async (enable) => {
        if (!room) return;
        try {
            await room.localParticipant.setScreenShareEnabled(enable);
            console.log(enable ? "✅ Screen share activé" : "🛑 Screen share désactivé");
        } catch (error) {
            console.error("❌ Erreur screen share:", error);
            // L'utilisateur a peut-être refusé la permission navigateur
            // On notifie Blazor pour remettre le bouton dans le bon état
            if (window.blazorInterop && window.blazorInterop.onScreenShareError) {
                window.blazorInterop.onScreenShareError(error.message);
            }
        }
    },

    leave: () => {
        if (room) {
            document.querySelectorAll('.participant-card.speaking').forEach(card => {
                card.classList.remove('speaking');
            });
            document.querySelectorAll('audio[data-livekit]').forEach(el => el.remove());
            document.querySelectorAll('video[data-livekit]').forEach(el => el.remove());

            room.disconnect();
            room = null;
            currentUserId = null;
            console.log("❌ Déconnecté");
        }
    },

    updateParticipantsList: (participants) => {
        console.log("📋 Liste mise à jour:", participants);
    },

    setParticipantVolume: (identity, volume) => {
        // Cherche d'abord par data-participant
        let audio = document.querySelector(`audio[data-participant="${identity}"]`);

        if (!audio) {
            // Fallback: cherche tous les audios et log pour débug
            const allAudios = document.querySelectorAll('audio[data-livekit]');
            console.warn(`⚠️ Pas d'audio trouvé pour "${identity}". Audios présents:`,
                [...allAudios].map(a => a.getAttribute('data-participant'))
            );
            return;
        }

        audio.volume = Math.max(0, Math.min(1, volume)); // sécurise entre 0 et 1
        console.log(`🔊 Volume de ${identity} : ${Math.round(volume * 100)}%`);
    }
};

// ─── Helpers ────────────────────────────────────────────────────────────────

function updateParticipantSpeaking(identity, isSpeaking) {
    const wrapper = document.getElementById(`participant-card-${identity}`);
    if (wrapper) {
        wrapper.classList.toggle('speaking', isSpeaking);
    }
}

// ✅ subscribeToParticipant gère maintenant AUDIO + VIDEO
function subscribeToParticipant(participant) {
    participant.trackPublications.forEach((publication) => {
        if (publication.track) {
            handleTrack(publication.track, participant);
        }
    });
}

function handleTrack(track, participant) {
    if (track.kind === "audio") {
        handleAudioTrack(track, participant);
    } else if (track.kind === "video") {
        handleVideoTrack(track, participant);
    }
}

function handleAudioTrack(track, participant) {
    console.log(`🎧 Audio de ${participant.identity}`);

    // Éviter les doublons
    const existing = document.querySelector(`audio[data-participant="${participant.identity}"]`);
    if (existing) return;

    const audioElement = track.attach();
    audioElement.setAttribute('data-livekit', 'true');
    audioElement.setAttribute('data-participant', participant.identity);
    document.body.appendChild(audioElement);

    audioElement.play().catch(err =>
        console.warn(`Autoplay bloqué pour ${participant.identity}:`, err)
    );
}

function handleVideoTrack(track, participant) {
    console.log(`🖥️ Vidéo/Screen share de ${participant.identity}`);

    // ✅ On cherche le conteneur dans le DOM Blazor
    const preview = document.getElementById(`screen-preview-${participant.identity}`);

    if (!preview) {
        console.warn(`⚠️ Pas de conteneur #screen-preview-${participant.identity} dans le DOM.`);
        console.warn("Assure-toi que ton composant Blazor rend un <div id='screen-preview-@identity'></div>");
        return;
    }

    // Éviter les doublons
    const existing = preview.querySelector('video[data-livekit]');
    if (existing) existing.remove();

    const videoElement = track.attach();
    videoElement.setAttribute('data-livekit', 'true');
    videoElement.setAttribute('data-participant', participant.identity);
    videoElement.style.cssText = `
        width: 100%;
        height: 100%;
        object-fit: contain;
        border-radius: 4px;
        background: #000;
    `;
    preview.appendChild(videoElement);
    preview.style.display = 'block';

    // ✅ Nettoyage quand la track se termine (ex: l'utilisateur clique "Arrêter le partage" dans le navigateur)
    track.on('ended', () => {
        console.log(`🛑 Track ended pour ${participant.identity}`);
        removeVideoTrack(participant.identity);
    });
}

function removeVideoTrack(identity) {
    const preview = document.getElementById(`screen-preview-${identity}`);
    if (preview) {
        preview.innerHTML = '';
        preview.style.display = 'none';
    }
    // Nettoyer aussi les éventuels éléments video orphelins
    document.querySelectorAll(`video[data-participant="${identity}"]`).forEach(el => el.remove());
}

function removeParticipantAudio(participant) {
    document.querySelectorAll(`audio[data-participant="${participant.identity}"]`).forEach(el => el.remove());
}