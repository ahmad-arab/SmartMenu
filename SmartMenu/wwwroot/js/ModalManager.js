class ModalManager {
    constructor(modalId = 'dynamicModal') {
        this.modalId = modalId;
        this.$modal = $('#' + modalId);
        this.$dialog = this.$modal.find('.modal-dialog');
        this.$content = this.$modal.find('.modal-content');
        this._callback = null;
        this._closeOnSuccess = true;
    }

    /**
     * Show the modal with content loaded from the given URL.
     * @param {string} url - The URL to load content from.
     * @param {string} size - Optional Bootstrap modal size class.
     * @param {string} title - Optional modal title.
     * @param {boolean} closeOnSuccess - Whether to close the modal on success (default: true).
     * @param {function} callback - Optional callback to call on success.
     */
    async show(url, size = '', title = '', closeOnSuccess = true, callback = null) {
        this.setSize(size);
        this._callback = callback;
        this._closeOnSuccess = closeOnSuccess;

        try {
            const html = await $.get(url);

            const contentHtml = `
                <div class="modal-header">
                    <h5 class="modal-title">${title ? title : ''}</h5>
                    <button type="button" class="btn border-0 bg-transparent p-0 ms-auto" data-bs-dismiss="modal" aria-label="Close" style="width: 1.5rem; height: 1.5rem;">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 16 16">
                            <path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="M4 4l8 8M12 4l-8 8"/>
                        </svg>
                    </button>
                </div>
                <div class="modal-body">
                    ${html}
                </div>
            `;
            this.$content.html(contentHtml);
            this.$modal.modal('show');

            // notify listeners that modal content is ready
            $(document).trigger('modal:content-ready', [this.$content]);

            this._wireForm();
        } catch (err) {
            this.$content.html('<div class="modal-body text-danger">Failed to load content.</div>');
            this.$modal.modal('show');
        }
    }

    setSize(size) {
        this.$dialog.removeClass('modal-sm modal-lg modal-xl');
        if (size) {
            this.$dialog.addClass(size);
        }
    }

    hide() {
        this.$modal.modal('hide');
    }

    _wireForm() {
        // Find the first form in the modal body
        const $form = this.$content.find('.modal-body form').first();
        if ($form.length === 0) return;

        // Remove any previous handler to avoid double binding
        $form.off('submit.modalmanager');

        $form.on('submit.modalmanager', (e) => {
            e.preventDefault();
            const formData = new FormData($form[0]);
            $.ajax({
                url: $form.attr('action'),
                type: $form.attr('method') || 'POST',
                data: formData,
                processData: false,
                contentType: false,
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (result, status, xhr) => {
                    const contentType = xhr.getResponseHeader('content-type');
                    if (contentType && contentType.indexOf('application/json') !== -1) {
                        // JSON response
                        if (result.success) {
                            this._showNotification('success', result.message || 'Success');
                            if (this._closeOnSuccess) {
                                this.hide();
                            }
                            if (typeof this._callback === 'function') {
                                this._callback(result);
                            }
                        } else {
                            this._showNotification('danger', result.message || 'Failed');
                            if (result.html) {
                                this.$content.find('.modal-body').html(result.html);
                                this._wireForm(); // re-bind for new form
                            }
                        }
                    } else {
                        // HTML response (validation errors)
                        this.$content.find('.modal-body').html(result);
                        this._wireForm(); // re-bind for new form
                    }
                },
                error: (xhr) => {
                    this._showNotification('danger', 'An error occurred while submitting the form.');
                }
            });
        });
    }

    _showNotification(type, message) {
        // Bootstrap 5 Toast (simple implementation)
        const toastId = 'modal-toast-' + Date.now();
        const isDark = type === 'success' || type === 'danger';
        const closeBtnClass = isDark ? 'btn-close btn-close-white' : 'btn-close';
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-bg-${type} border-0 position-fixed top-0 end-0 m-3" role="alert" aria-live="assertive" aria-atomic="true" style="z-index: 2000;">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="${closeBtnClass} btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        const $toast = $(toastHtml).appendTo('body');
        const toast = new bootstrap.Toast($toast[0], { delay: 3000 });
        toast.show();
        $toast.on('hidden.bs.toast', function () { $toast.remove(); });
    }
}


window.modalManager = new ModalManager('dynamicModal');
window.modalManager2 = new ModalManager('dynamicModal2');
$('#dynamicModal2').on('show.bs.modal', function () {
    setTimeout(function () {
        $('.modal-backdrop').last().addClass('level2-backdrop');
    }, 10);
});