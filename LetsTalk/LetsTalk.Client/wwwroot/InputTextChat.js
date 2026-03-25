window.preventDefaultOnActiveElement = () => {
    const el = document.activeElement;
    if (el && el.tagName === 'TEXTAREA') {
        el.value = '';
        el.dispatchEvent(new Event('input', { bubbles: true }));
    }
};