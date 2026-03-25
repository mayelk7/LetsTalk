window.preventEnterDefault = (wrapperElement) => {
    const textarea = wrapperElement.querySelector('textarea');
    if (textarea) {
        textarea.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
            }
        });
    }
};