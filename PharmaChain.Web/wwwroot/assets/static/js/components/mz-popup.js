const MzPopup = (() => {
    const ICONS = { success: 'bi-check-lg', danger: 'bi-x-lg', warning: 'bi-exclamation-lg', info: 'bi-info-lg', primary: 'bi-shield-check' };
    let _onOk = null, _onCancel = null, _closeOnOverlay = true;
    const overlay = document.getElementById('mzPopupOverlay');
    const box = document.getElementById('mzPopupBox');
    const iconEl = document.getElementById('mzPopupIconInner');
    const titleEl = document.getElementById('mzPopupTitle');
    const msgEl = document.getElementById('mzPopupMessage');
    const okBtn = document.getElementById('mzPopupOk');
    const cancelBtn = document.getElementById('mzPopupCancel');

    function show({ type = 'primary', title = '', message = '', okText = 'OK', cancelText = null, onOk = null, onCancel = null, closeOnOverlay = true } = {}) {
        box.className = `mz-popup ${type}`;
        iconEl.className = `bi ${ICONS[type] || ICONS.primary}`;
        titleEl.textContent = title;
        msgEl.innerHTML = message;
        okBtn.textContent = okText;
        if (cancelText) { cancelBtn.textContent = cancelText; cancelBtn.style.display = ''; }
        else { cancelBtn.style.display = 'none'; }
        _onOk = onOk; _onCancel = onCancel; _closeOnOverlay = closeOnOverlay;
        overlay.classList.add('open'); okBtn.focus();
    }

    function close() { overlay.classList.remove('open'); }
    function _handleOk() { close(); if (typeof _onOk === 'function') _onOk(); }
    function _handleCancel() { close(); if (typeof _onCancel === 'function') _onCancel(); }
    function _handleOverlayClick(e) { if (_closeOnOverlay && e.target === overlay) close(); }

    document.addEventListener('keydown', e => { if (e.key === 'Escape' && overlay.classList.contains('open')) close(); });

    return { show, close, _handleOk, _handleCancel, _handleOverlayClick };
})();