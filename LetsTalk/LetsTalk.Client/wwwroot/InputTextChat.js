window.registerChatInput = (dotnetRef, elementId) => {
    const el = document.getElementById(elementId)?.querySelector('textarea');
    if (!el) return;
    el.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            dotnetRef.invokeMethodAsync('SendMessage');
        }
    });
};