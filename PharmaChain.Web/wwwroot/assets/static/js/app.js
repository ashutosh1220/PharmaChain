
$(function() {

    var $toggleBtn = $('#sidebarToggleBtn');
    var $toggleIcon = $('#sidebarToggleIcon');
    var $sidebar = $('#sidebar');
    var $sidebarInner = $('.sidebar-wrapper');
    var $overlay = $('#sidebarOverlay');

    if (!$toggleBtn.length || !$sidebar.length) return;

    function isDesktop() { return $(window).width() >= 1200; }
    function isSidebarOpen() { return !$sidebar.hasClass("collapsed");}
    function openSidebar() {
        $sidebar.removeClass("collapsed");
        if ($sidebarInner.length) $sidebarInner.addClass("active");
        if ($toggleIcon.length) $toggleIcon.attr("class", "bi bi-layout-sidebar");
        if (!isDesktop() && $overlay.length) {
            $overlay.addClass("show");
            $('.sidebar-wrapper').addClass('open');
        }
    }

    function closeSidebar() {
        $sidebar.addClass("collapsed");
        if ($sidebarInner.length) $sidebarInner.removeClass("active");
        if ($toggleIcon.length) $toggleIcon.attr("class", "bi bi-list");
        if ($overlay.length) {
            $overlay.removeClass("show");
            $('.sidebar-wrapper').removeClass('open');
        }
    }

    $toggleBtn.on("click", function(e) {
        e.stopPropagation();
        isSidebarOpen() ? closeSidebar() : openSidebar();
    });

    if ($overlay.length) $overlay.on("click", closeSidebar);

    function handleResize() {
        if (isDesktop()) {
            openSidebar();
            if ($overlay.length) $overlay.removeClass("show");
        } else {
            closeSidebar();
        }
    }

    handleResize();
    $(window).on("resize", handleResize);
});

const ICON_MAP = {
    primary: 'fa-circle-info',
    success: 'fa-circle-check',
    danger: 'fa-circle-xmark',
    warning: 'fa-triangle-exclamation',
    info: 'fa-circle-question',
    dark: 'fa-bell',
};

function showToast(opts) {
    const {
        type = 'primary',
        title = '',
        message = '',
        icon,
        duration = 4000,
        variant,
        compact = false,
        actions = [],
    } = opts;

    var $container = $('#toast-container');

    var classes = ['mz-toast', 'toast-' + type];
    if (variant) classes.push(variant);
    if (compact) classes.push('compact');

    var iconClass = icon || ICON_MAP[type] || 'fa-circle-info';
    var timeStr = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    var actionsHTML = '';
    if (actions.length) {
        actionsHTML = '<div class="mz-toast-actions">' +
            actions.map(a =>
                `<button class="mz-toast-action-btn ${a.cls}">${a.label}</button>`
            ).join('') +
            '</div>';
    }

    var showMsg = message && !compact;
    var showTime = !compact && variant !== 'minimal';

    var toastHTML = `
    <div class="${classes.join(' ')}">
      <div class="mz-toast-inner">
        <div class="mz-toast-icon"><i class="fa-solid ${iconClass}"></i></div>
        <div class="mz-toast-body">
          <div class="mz-toast-title">${title}</div>
          ${showMsg ? `<div class="mz-toast-message">${message}</div>` : ''}
          ${compact && message ? `<div class="mz-toast-message">${message}</div>` : ''}
          ${showTime ? `<div class="mz-toast-time"><i class="fa-regular fa-clock"></i>${timeStr}</div>` : ''}
        </div>
        <button class="mz-toast-close">
          <i class="fa-solid fa-xmark"></i>
        </button>
      </div>
      ${actionsHTML}
      ${duration > 0 ? `<div class="mz-toast-progress">
        <div class="mz-toast-progress-bar" style="animation-duration:${duration}ms"></div>
      </div>` : ''}
    </div>
  `;

    var $toast = $(toastHTML);

    $container.append($toast);

    $toast.find('.mz-toast-close').on('click', function() {
        removeToast($toast);
    });

    $toast.find('.mz-toast-action-btn').each(function(i) {
        if (actions[i] && actions[i].onClick) {
            $(this).on('click', actions[i].onClick);
        }
        $(this).on('click', function() {
            removeToast($toast);
        });
    });

    if (duration > 0) {
        setTimeout(function() {
            removeToast($toast);
        }, duration);
    }
}

function removeToast($toast) {
    if (!$toast || $toast.hasClass('removing')) return;

    $toast.addClass('removing');

    setTimeout(function() {
        $toast.remove();
    }, 300);
}

let currentPos = 'pos-top-right';

function setPos(pos) {
    var $container = $('#toast-container');

    $container.removeClass().addClass(pos);

    currentPos = pos;

    $('.pos-btn').removeClass('active');
    $(event.target).addClass('active');
}

function toggleTheme() {
    var $html = $('html');

    var current = $html.attr('data-bs-theme');
    $html.attr('data-bs-theme', current === 'dark' ? 'light' : 'dark');
}

function initRegistrationWizard() {
    if (!$('#nextBtn').length) return;

    const totalSteps = 5;
    let currentStep = 1;
    let emailVerified = false;
    let otpTimerInterval = null;
    let currentCaptcha = '';

    const stepIcons = [
        'bi-person-fill', 'bi-geo-alt-fill', 'bi-shield-lock-fill',
        'bi-key-fill', 'bi-card-image'
    ];

    function updateWizard() {
        for (let i = 1; i <= totalSteps; i++) {
            var $content = $('#step-' + i);
            var $circle = $('#circle-' + i);
            var $label = $('#label-' + i);
            if (!$content.length || !$circle.length || !$label.length) continue;
            $content.addClass('d-none');
            $circle.removeClass('active completed');
            $label.removeClass('active completed');
            if (i < currentStep) {
                $circle.addClass('completed').html('<i class="bi bi-check-lg"></i>');
                $label.addClass('completed');
            } else if (i === currentStep) {
                $circle.addClass('active').html('<i class="bi ' + stepIcons[i - 1] + '"></i>');
                $label.addClass('active');
                $content.removeClass('d-none');
            } else {
                $circle.html('<i class="bi ' + stepIcons[i - 1] + '"></i>');
            }
        }
        var progress = ((currentStep - 1) / (totalSteps - 1)) * 90;
        $('#stepProgressLine').css('width', progress + '%');
        $('#prevBtn').prop('disabled', currentStep === 1);
        if (currentStep === totalSteps) {
            $('#nextBtn').html('<i class="bi bi-check-circle"></i> Submit').attr('class', 'btn btn-success');
        } else {
            $('#nextBtn').html('Next <i class="bi bi-arrow-right"></i>').attr('class', 'btn btn-primary');
        }
    }

    function showError(id, msg) {
        var $el = $('#' + id);
        var $input = $('#' + id.replace('err-', ''));
        if ($el.length) { if (msg) $el.text(msg); $el.show(); }
        if ($input.length) $input.addClass('is-invalid-custom');
    }

    function clearError(id) {
        $('#' + id).hide();
        $('#' + id.replace('err-', '')).removeClass('is-invalid-custom');
    }

    function clearAllErrors() {
        $('.field-error').hide();
        $('.is-invalid-custom').removeClass('is-invalid-custom');
    }

    function parseError(xhr, fallback) {
        try {
            var resp = JSON.parse(xhr.responseText);
            return resp.message || resp.Message || resp.error || resp.Error || resp.title || resp.Title || fallback;
        } catch (e) { return fallback; }
    }

    function validateStep1() {
        var valid = true; clearAllErrors();
        if (!$('#fname').val().trim()) { showError('err-fname'); valid = false; }
        if (!/^[0-9]{10}$/.test($('#mobile').val().trim())) { showError('err-mobile'); valid = false; }
        var email = $('#email').val().trim();
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            showError('err-email', 'Enter a valid email address.'); valid = false;
        } else if (!emailVerified) {
            showError('err-email', 'Please verify your email with OTP before proceeding.'); valid = false;
        }
        if (!$('input[name="Gender"]:checked').length) { showError('err-gender'); valid = false; }
        $('#username').val($('#email').val());
        return valid;
    }

    function validateStep2() {
        var valid = true; clearAllErrors();
        if (!$('#addressLine1').val().trim()) { showError('err-addressLine1'); valid = false; }
        if (!$('#country').val()) { showError('err-country'); valid = false; }
        if (!$('#state').val()) { showError('err-state'); valid = false; }
        if (!$('#city').val().trim()) { showError('err-city'); valid = false; }
        if (!/^[0-9]{6}$/.test($('#pincode').val().trim())) { showError('err-pincode'); valid = false; }
        return valid;
    }

    function validateStep3() {
        var valid = true; clearAllErrors();
        if (!$('#branchSelect').val()) { showError('err-branch'); valid = false; }
        if (!$('#roleSelect').val()) { showError('err-role'); valid = false; }
        return valid;
    }

    function validateStep4() {
        var valid = true; clearAllErrors();
        var pass = $('#password').val();
        if (pass.length < 8) { showError('err-password'); valid = false; }
        var confirm = $('#confirmPassword').val();
        if (!confirm || pass !== confirm) { showError('err-confirmPassword'); valid = false; }
        return valid;
    }

    function validateStep5() {
        var valid = true; clearAllErrors();
        if (!$('#idProofType').val()) { showError('err-idProofType'); valid = false; }
        if (!$('#idProofNumber').val().trim()) { showError('err-idProofNumber'); valid = false; }
        if (!$('#idProofDoc')[0].files.length) { showError('err-idProofDoc'); valid = false; }
        if (!$('#profileImage')[0].files.length) { showError('err-profileImage'); valid = false; }
        var entered = $('#captchaInput').val().trim().toUpperCase();
        if (!entered || entered !== currentCaptcha) {
            showError('err-captcha', 'Captcha does not match. Please try again.');
            generateCaptcha(); $('#captchaInput').val(''); valid = false;
        }
        if (!$('#consentCheck').is(':checked')) { showError('err-consent'); valid = false; }
        return valid;
    }

    var validators = [null, validateStep1, validateStep2, validateStep3, validateStep4, validateStep5];

    $('#nextBtn').off('click').on('click', function() {
        if (!validators[currentStep]()) return;
        if (currentStep < totalSteps) { currentStep++; updateWizard(); }
        else { $('#submitBtn').trigger('click'); }
    });

    $('#prevBtn').off('click').on('click', function() {
        if (currentStep > 1) { currentStep--; updateWizard(); }
    });

    function formatTime(seconds) {
        var m = Math.floor(seconds / 60), s = seconds % 60;
        return m + ':' + (s < 10 ? '0' + s : s);
    }

    function startOtpTimer(seconds) {
        clearInterval(otpTimerInterval);
        var remaining = seconds;
        $('#sendOtpBtn').html('<i class="bi bi-clock"></i> Resend in ' + formatTime(remaining));
        $('#otpTimer').text('Resend OTP in ' + formatTime(remaining));
        otpTimerInterval = setInterval(function() {
            remaining--;
            $('#sendOtpBtn').html('<i class="bi bi-clock"></i> Resend in ' + formatTime(remaining));
            $('#otpTimer').text('Resend OTP in ' + formatTime(remaining));
            if (remaining <= 0) {
                clearInterval(otpTimerInterval);
                $('#otpTimer').text('');
                $('#sendOtpBtn').prop('disabled', false).html('<i class="bi bi-envelope-fill"></i> Resend OTP');
            }
        }, 1000);
    }

    $('#sendOtpBtn').off('click').on('click', function() {
        clearError('err-email');
        var email = $('#email').val().trim();
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            showError('err-email', 'Enter a valid email before sending OTP.'); return;
        }
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Sending...');
        showOverlay('primary', 'Sending OTP…');
        $.ajax({
            url: '/api/send-otp', type: 'POST', data: { email: email },
            success: function() {
                $('#otpLabelCol').removeClass('d-none');
                $('#otpFieldCol').removeClass('d-none');
                $('#emailVerifiedBadge').addClass('d-none');
                startOtpTimer(120);
            },
            error: function(xhr) {
                showError('err-email', parseError(xhr, 'Failed to send OTP. Please try again.'));
                $btn.prop('disabled', false).html('<i class="bi bi-envelope-fill"></i> Send OTP');
            },
            complete: function() {
                hideOverlay();
            }
        });
    });

    $('#verifyOtpBtn').off('click').on('click', function() {
        clearError('err-otp');
        var entered = $('#otpInput').val().trim();
        if (entered.length !== 6) { showError('err-otp', 'Enter the 6-digit OTP.'); return; }
        var email = $('#email').val().trim();
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Verifying...');
        showOverlay('info', 'Verifying OTP…');
        $.ajax({
            url: '/api/verify-otp', type: 'POST', data: { Email: email, Otp: entered },
            success: function() {
                emailVerified = true;
                clearInterval(otpTimerInterval);
                $('#otpTimer').text('');
                $('#otpLabelCol').addClass('d-none');
                $('#otpFieldCol').addClass('d-none');
                $('#emailVerifiedBadge').removeClass('d-none');
                $('#email').prop('readonly', true);
                $('#sendOtpBtn').prop('disabled', true)
                    .html('<i class="bi bi-check-circle-fill" style="position: relative; bottom: 120px; left: -5px;"></i> Verified')
                    .attr('class', 'btn btn-success');
            },
            error: function(xhr) {
                showError('err-otp', parseError(xhr, 'Incorrect OTP. Please try again.'));
                $btn.prop('disabled', false).html('<i class="bi bi-patch-check-fill" style="position: relative; bottom: 12px; left: -5px;"></i> Verify OTP');
            },
            complete: function() {
                hideOverlay();
            }
        });
    });

    function generateCaptcha() {
        var chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
        currentCaptcha = '';
        for (var c = 0; c < 6; c++) currentCaptcha += chars[Math.floor(Math.random() * chars.length)];
        $('#captchaText').text(currentCaptcha);
    }

    $('#refreshCaptcha').off('click').on('click', function() {
        generateCaptcha(); $('#captchaInput').val(''); clearError('err-captcha');
    });

    function toggleVisibility(inputId, iconId) {
        var $input = $('#' + inputId);
        $input.attr('type', $input.attr('type') === 'password' ? 'text' : 'password');
        $('#' + iconId).toggleClass('bi-eye bi-eye-slash');
    }

    $('#togglePassword').off('click').on('click', function() { toggleVisibility('password', 'togglePasswordIcon'); });
    $('#toggleConfirmPassword').off('click').on('click', function() { toggleVisibility('confirmPassword', 'toggleConfirmPasswordIcon'); });

    $('#confirmPassword').off('input').on('input', function() {
        var match = $(this).val() === $('#password').val();
        $('#passwordMatchMsg')
            .text(match ? '✓ Passwords match.' : '✗ Passwords do not match.')
            .attr('class', match ? 'text-success' : 'text-danger').show();
    });

    $('#profileImage').off('change').on('change', function() {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function(e) { $('#profilePreview').attr('src', e.target.result).removeClass('d-none'); };
            reader.readAsDataURL(file);
        }
    });

    updateWizard();
    generateCaptcha();
}

$(document).ready(function() {
    initRegistrationWizard();
});


var totalPages = 1;
var currentPage = 1;
var pageSize = 10;
var allData = [];
var filteredData = [];
var currentSort = { field: 'createdAt', direction: 'desc' };

function getCurrentRoute() {
    var path = window.location.pathname;
    return path.split('?')[0];
}

function getUrlParams() {
    var params = new URLSearchParams(window.location.search);
    return {
        page: parseInt(params.get('page')) || 1,
        size: parseInt(params.get('size')) || 10,
        search: params.get('search') || '',
        filter: params.get('filter') || '',
        sort: params.get('sort') || 'createdAt-desc'
    };
}

function buildQueryString(page = 1, size = pageSize, search = '', filter = '') {
    var params = new URLSearchParams();
    params.set('page', page);
    params.set('size', size);
    if (search) params.set('search', search);
    if (filter) params.set('filter', filter);
    return params.toString();
}

function renderPagination() {
    var $pagination = $('#tableFooter').find('.pagination');
    if (!$pagination.length) return;

    $pagination.empty();

    var prevPage = Math.max(1, currentPage - 1);
    $pagination.append(
        `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${prevPage}">Prev</a>
        </li>`
    );

    if (totalPages <= 7) {
        for (var i = 1; i <= totalPages; i++) {
            $pagination.append(
                `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>`
            );
        }
    } else if (currentPage <= 4) {
        for (var i = 1; i <= 7; i++) {
            $pagination.append(
                `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>`
            );
        }
        $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        $pagination.append(`<li class="page-item"><a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a></li>`);
    } else if (currentPage >= totalPages - 3) {
        $pagination.append(`<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`);
        $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        for (var i = totalPages - 6; i <= totalPages; i++) {
            $pagination.append(
                `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>`
            );
        }
    } else {
        $pagination.append(`<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`);
        $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        for (var i = currentPage - 2; i <= currentPage + 2; i++) {
            $pagination.append(
                `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>`
            );
        }
        $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        $pagination.append(`<li class="page-item"><a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a></li>`);
    }

    var nextPage = Math.min(totalPages, currentPage + 1);
    $pagination.append(
        `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${nextPage}">Next</a>
        </li>`
    );
}

$(document).ready(function() {

    if (!$('#tableFooter').length) return;

    var $footer = $('#tableFooter');
    totalPages = parseInt($footer.data('total-pages')) || 1;
    currentPage = parseInt($footer.data('current-page')) || 1;
    pageSize = parseInt($footer.data('page-size')) || 10;

    renderPagination();

    $('.col-toggle').on('change', function() {
        var colClass = $(this).data('col');
        $(this).is(':checked') ? $('.' + colClass).show() : $('.' + colClass).hide();
    });

    $('.col-toggle').each(function() {
        if (!$(this).is(':checked')) $('.' + $(this).data('col')).hide();
    });
});

let overlayTimer;

function showOverlay(color, label) {
    const el = document.getElementById('mzPageLoader');
    const inner = document.getElementById('mzPageLoaderInner');
    const lbl = document.getElementById('mzPageLoaderLabel');
    inner.className = `mz-loader mz-loader-lg mz-loader-${color}`;
    lbl.textContent = label;
    el.classList.add('show');
    clearTimeout(overlayTimer);
    overlayTimer = setTimeout(hideOverlay, 2500);
}

function hideOverlay() {
    document.getElementById('mzPageLoader').classList.remove('show');
}

function simulateBtn(id, loadingText) {
    const btn = document.getElementById(id);
    const origHTML = btn.innerHTML;
    btn.classList.add('mz-btn-loading');
    btn.innerHTML = `<span class="mz-btn-spinner"></span> ${loadingText}`;
    setTimeout(() => { btn.classList.remove('mz-btn-loading'); btn.innerHTML = origHTML; }, 2200);
}

$(document).on('click', '.ajax-link', function(e) {
    e.preventDefault();
    var url = $(this).attr('data-url') || $(this).attr('href');

    $('#mzPageLoader').addClass('show');

    $.ajax({
        url: url,
        method: 'GET',
        success: function(response) {
            var title = $(response).filter('title').text() || $(response).find('title').text();
            var content = $(response).find('#mainContent').html();
            if (!content) content = response;

            $('#mainContent').html(content);
            history.pushState(null, title, url);

            if (title) {
                document.title = title;
                $('.top-bar-title').text(title);
            }

            if ($('#tableFooter').length) {
                var $footer = $('#tableFooter');
                totalPages = parseInt($footer.attr('data-total-pages')) || 1;
                currentPage = parseInt($footer.attr('data-current-page')) || 1;
                pageSize = parseInt($footer.attr('data-page-size')) || 10;
                renderPagination();
            }

            if ($('#userTableBody').length) {
                UsersPage.init();
            }

            if ($('#logPage').length) {
                LogsPage.init();
                renderPagination();
            }

            if ($('#nextBtn').length) {
                initRegistrationWizard();
            }

            if ($('#managePermissionsPage').length) {
                buildPermGrid();
            }

            if ($('#batchTableBody').length) {
                BatchPage.init();
            }

            if ($('#medicineTableBody').length) {
                MedicinePage.init();
            }

            if ($('#etTableBody').length) {
                ExpiryPage.init();
            }
        },
        error: function() {
            showToast({ type: 'danger', title: 'Error', message: 'Page load karne mein error aaya.' });
        },
        complete: function() {
            $('#mzPageLoader').removeClass('show');
        }
    });
});

function confirmAction(options) {
    MzPopup.show({
        type: options.type || 'warning',
        title: options.title || 'Are you sure?',
        message: options.message || 'Please confirm this action.',
        okText: options.okText || 'Yes',
        cancelText: options.cancelText || 'Cancel',
        onOk: function() {
            if (!options.url) return;
            $.ajax({
                url: options.url,
                type: options.method || 'POST',
                success: function(res) {
                    if (res.success) {
                        if (options.onSuccess) { options.onSuccess(res); }
                        else { MzPopup.show({ type: 'success', title: 'Success', message: res.message || 'Action completed', okText: 'OK' }); }
                    } else {
                        MzPopup.show({ type: 'danger', title: 'Error', message: res.message || 'Failed', okText: 'OK' });
                    }
                },
                error: function(xhr) {
                    let errorMsg = "Something went wrong";
                    try { let res = JSON.parse(xhr.responseText); errorMsg = res.message || errorMsg; } catch (e) { }
                    MzPopup.show({ type: 'danger', title: 'Error', message: errorMsg, okText: 'OK' });
                }
            });
        }
    });
}

// FORM SUBMIT + USER ACTIONS

$(document).ready(function() {

    //$(document).on('submit', 'form', function(e) {
    //    e.preventDefault();
    //    var form = this;
    //    var formData = new FormData(form);
    //    var confirmMessage = $(form).data('confirm');

    //    function submitForm() {
    //        $.ajax({
    //            url: $(form).attr('action'),
    //            type: 'POST',
    //            data: formData,
    //            processData: false,
    //            contentType: false,

    //            success: function(response) {
    //                let msg = response.message || response.messege || "No message";

    //                if (response.success) {
    //                    MzPopup.show({
    //                        type: 'success',
    //                        title: 'Success',
    //                        message: msg,
    //                        okText: 'OK',
    //                        onOk: function() {
    //                            form.reset();
    //                            window.location.reload();
    //                        }
    //                    });
    //                } else {
    //                    MzPopup.show({
    //                        type: 'danger',
    //                        title: 'Failed',
    //                        message: msg,
    //                        okText: 'OK'
    //                    });
    //                }
    //            },

    //            error: function(xhr) {
    //                let errorMsg = "Something went wrong";

    //                try {
    //                    let res = JSON.parse(xhr.responseText);
    //                    errorMsg = res.message || res.messege || errorMsg;
    //                } catch (e) { }

    //                MzPopup.show({
    //                    type: 'danger',
    //                    title: 'Error',
    //                    message: errorMsg,
    //                    okText: 'OK'
    //                });
    //            }
    //        });
    //    }

    //    if (confirmMessage) {
    //        MzPopup.show({
    //            type: 'warning', title: 'Confirm Action', message: confirmMessage, okText: 'Yes', cancelText: 'Cancel',
    //            onOk: function() { submitForm(); }
    //        });
    //    } else {
    //        submitForm();
    //    }
    //});


    $(document).on('submit', 'form', function (e) {
        e.preventDefault();
        var form = this;
        var formData = new FormData(form);
        var confirmMessage = $(form).data('confirm');

        var dataLog = {};
        for (let [key, value] of formData.entries()) {
            console.log(`  ${key}: ${value}`);
            dataLog[key] = value;
        }

        let payload;
        let isStockRequestForm = (form.id === 'srq-srqForm');

        if (isStockRequestForm) {

            let medicinesMap = {};

            for (let [key, value] of formData.entries()) {

                let match = key.match(/^Medicines\[(\d+)\]\.(\w+)$/);

                if (match) {
                    let index = match[1];
                    let field = match[2];

                    if (!medicinesMap[index]) medicinesMap[index] = {};
                    medicinesMap[index][field] = value;
                }
            }

            let medicines = Object.values(medicinesMap).map(x => ({
                medicineId: x.MedicineId,
                batchId: x.BatchId || "",
                quantity: parseInt(x.Quantity || 0)
            }));

            payload = {
                transferId: "",
                assignedStockId: "",
                medicines: medicines
            };

        } else {
            payload = formData;
        }

        function submitForm() {
            $.ajax({
                url: $(form).attr('action'),
                type: 'POST',
                data: isStockRequestForm ? JSON.stringify(payload) : formDat,
                processData: !isStockRequestForm,
                contentType: isStockRequestForm ? 'application/json; charset=utf-8' : false,

                success: function (response) {
                    console.log('Server Response:', response);

                    let msg = response.message || response.messege || "No message";

                    if (response.success) {
                        MzPopup.show({
                            type: 'success',
                            title: 'Success',
                            message: msg,
                            okText: 'OK',
                            onOk: function () {
                                // Reset form state
                                form.reset();

                                if (window.selectedBranch !== undefined) {
                                    window.selectedBranch = null;
                                }
                                if (window.selectedMedicines !== undefined) {
                                    window.selectedMedicines = [];
                                }
                                if (window.updateTags !== undefined) {
                                    window.updateTags();
                                }
                                if (window.renderHiddenFields !== undefined) {
                                    window.renderHiddenFields();
                                }
                                window.location.reload();
                            }
                        });
                    } else {
                        MzPopup.show({
                            type: 'danger',
                            title: 'Failed',
                            message: msg,
                            okText: 'OK'
                        });
                    }
                },

                error: function (xhr) {
                    console.error(' Error:', xhr);

                    let errorMsg = "Something went wrong";

                    try {
                        let res = JSON.parse(xhr.responseText);
                        errorMsg = res.message || res.messege || errorMsg;
                    } catch (e) {
                        errorMsg = "Server error: " + xhr.status + " " + xhr.statusText;
                    }

                    MzPopup.show({
                        type: 'danger',
                        title: 'Error',
                        message: errorMsg,
                        okText: 'OK'
                    });
                }
            });
        }

        if (confirmMessage) {
            MzPopup.show({
                type: 'warning',
                title: 'Confirm Action',
                message: confirmMessage,
                okText: 'Yes',
                cancelText: 'Cancel',
                onOk: function () {
                    submitForm();
                }
            });
        } else {
            submitForm();
        }
    });


    $(document).on("click", ".toggle-status", function() {
        let row = $(this).closest("tr");
        let userId = row.find("td:eq(1) small").text().trim();
        confirmAction({
            title: "Change Status", message: "Do you want to toggle this user's status?",
            url: "/api/toggle-user-status?id=" + encodeURIComponent(userId), method: "PATCH",
            onSuccess: function(res) {
                let badge = row.find("td:eq(6) span");
                if (res.data) { badge.removeClass("bg-danger").addClass("bg-success").text("Active"); }
                else { badge.removeClass("bg-success").addClass("bg-danger").text("Inactive"); }
                MzPopup.show({ type: 'success', title: 'Updated', message: 'User status updated', okText: 'OK' });
            }
        });
    });

    $(document).on("click", ".delete-user", function() {
        let row = $(this).closest("tr");
        let userId = row.find("td:eq(1) small").text().trim();
        confirmAction({
            title: "Delete User", message: "Are you sure you want to delete this user?",
            url: "/api/delete-user?id=" + encodeURIComponent(userId), method: "DELETE",
            onSuccess: function(res) {
                row.remove();
                MzPopup.show({ type: 'success', title: 'Deleted', message: 'User deleted successfully', okText: 'OK' });
            }
        });
    });
});


var UsersPage = {
    state: {
        page: 1, size: 10, search: '',
        role: '', branch: '',
        sort: 'createdAt-desc', status: 'All'
    },

    fetch: function() {
        $('#mzPageLoader').addClass('show');
        $.ajax({
            url: '/Users-List',
            method: 'GET',
            data: UsersPage.state,
            success: function(response) {
                var $res = $(response);

                var newBody = $res.find('#userTableBody').html();
                if (newBody) $('#userTableBody').html(newBody);

                var $newFooter = $res.find('#tableFooter');
                if ($newFooter.length) {
                    $('#tableFooter')
                        .data('total-pages', $newFooter.data('total-pages'))
                        .data('current-page', $newFooter.data('current-page'))
                        .data('page-size', $newFooter.data('page-size'));
                }

                var newStats = $res.find('#statCards').html();
                if (newStats) $('#statCards').html(newStats);

                totalPages = parseInt($('#tableFooter').data('total-pages')) || 1;
                currentPage = UsersPage.state.page;
                pageSize = parseInt(UsersPage.state.size);

                renderPagination();
                UsersPage.updateResultsInfo();

                history.pushState(null, '', '/Users-List?' + $.param(UsersPage.state));
            },
            error: function() {
                showToast({ type: 'danger', title: 'Error', message: 'Users load karne mein error aaya.' });
            },
            complete: function() {
                $('#mzPageLoader').removeClass('show');
            }
        });
    },

    bindRowsPerPage: function() {
        // Now handled by delegated event handler at document level
    },

    updateResultsInfo: function() {
        var showing = $('#userTableBody tr').length;
        var tp = parseInt($('#tableFooter').data('total-pages')) || 1;
        var total = tp * parseInt(UsersPage.state.size);
        $('#resultsInfo').text('Showing ' + showing + ' of ' + total + ' users');
    },

    resetFilters: function() {
        UsersPage.state = {
            page: 1, size: 10, search: '',
            role: '', branch: '',
            sort: 'createdAt-desc', status: 'All'
        };
        $('#searchInput').val('');
        $('#filterRole').val('');
        $('#filterBranch').val('');
        $('#sortField').val('createdAt-desc');
        UsersPage.fetch();
    },

    init: function() {
        UsersPage.state.page = parseInt($('#tableFooter').data('current-page')) || 1;
        UsersPage.state.size = parseInt($('#tableFooter').data('page-size')) || 10;
        UsersPage.updateResultsInfo();
        // pagination click is handled by the single delegated handler below
    }
};

$(function() {
    if ($('#userTableBody').length) {
        UsersPage.init();
    }
});

function exportExcel() {
    var rows = [];

    rows.push(['Name', 'User ID', 'Phone', 'Email', 'Role', 'Branch', 'Status', 'Created At']);

    $('#userTableBody tr').each(function() {
        var $tds = $(this).find('td');
        rows.push([
            $tds.eq(1).find('strong').text().trim(),
            $tds.eq(1).find('small').text().trim(),
            $tds.eq(2).text().trim(),
            $tds.eq(3).text().trim(),
            $tds.eq(4).text().trim(),
            $tds.eq(5).text().trim(),
            $tds.eq(6).text().trim(),
            $tds.eq(7).text().trim(),
        ]);
    });

    var wb = XLSX.utils.book_new();
    var ws = XLSX.utils.aoa_to_sheet(rows);

    ws['!cols'] = [
        { wch: 20 },
        { wch: 15 },
        { wch: 15 },
        { wch: 28 },
        { wch: 15 },
        { wch: 15 },
        { wch: 12 },
        { wch: 15 },
    ];

    XLSX.utils.book_append_sheet(wb, ws, 'Users');
    var date = new Date();
    var fileName = 'Users_' +
        date.getFullYear() + '-' +
        String(date.getMonth() + 1).padStart(2, '0') + '-' +
        String(date.getDate()).padStart(2, '0') + '.xlsx';

    XLSX.writeFile(wb, fileName);

    showToast({ type: 'success', title: 'Exported', message: 'Excel exported successfully.' });
}


$(document).on('click', '.fsb', function() {
    var $btn = $(this);
    $btn.prop('disabled', true);

    var $input = $btn.closest('.fr').find('input, select, textarea');

    var id = $('#userId').val();
    var columnName = $input.data('field');
    var value = $input.val();
    var dispId = $input.data('disp');

    var payload = {
        userId: id,
        columnName: columnName,
        updatedValue: value
    };

    $.ajax({
        url: '/api/update-user-info',
        type: 'PATCH',
        contentType: 'application/json',
        data: JSON.stringify(payload),

        success: function(res) {
            if (dispId) {
                $('#' + dispId).text(value);
            }

            console.log("SUCCESS HIT");

            showToast({
                type: 'success',
                title: 'Updated',
                message: 'Field updated successfully',
                icon: 'fa-circle-check'
            });
        },

        error: function(xhr) {
            console.log(xhr.responseText);

            showToast({
                type: 'danger',
                title: 'Error',
                message: 'Update failed',
                icon: 'fa-circle-xmark'
            });
        },

        complete: function() {
            $btn.prop('disabled', false);
        }
    });
});

const ROLES = [
    { id: 'SuperAdmin', label: 'Super Admin', icon: 'bi-stars' },
    { id: 'Admin', label: 'Admin', icon: 'bi-shield-fill' },
    { id: 'BranchManager', label: 'Branch Manager', icon: 'bi-building-fill' },
    { id: 'StockManager', label: 'Stock Manager', icon: 'bi-box-seam-fill' },
    { id: 'Pharmacist', label: 'Pharmacist', icon: 'bi-capsule-pill' },
    { id: 'Cashier', label: 'Cashier', icon: 'bi-cash-coin' },
    { id: 'Auditor', label: 'Auditor', icon: 'bi-clipboard-data-fill' },
];

const GROUP_ICONS = {
    Dashboard: 'bi-grid-fill', Users: 'bi-person-circle', Roles: 'bi-shield-lock-fill',
    Permissions: 'bi-shield-check', Security: 'bi-shield-fill-check', ActivityLogs: 'bi-clipboard-data-fill',
    Medicines: 'bi-capsule-pill', Batches: 'bi-box-seam-fill', Suppliers: 'bi-truck-front-fill',
    StockTransfers: 'bi-arrow-left-right', StockLedger: 'bi-journal-text', Customers: 'bi-people-fill',
    SalesInvoices: 'bi-receipt-cutoff', Returns: 'bi-arrow-return-left', Payments: 'bi-cash-coin',
    PurchaseInvoices: 'bi-file-earmark-text-fill', Branches: 'bi-building-fill',
    SalesReports: 'bi-graph-up-arrow', PurchaseReports: 'bi-cart-check-fill',
    StockReports: 'bi-box-seam', FinancialReports: 'bi-bar-chart-fill',
    Settings: 'bi-gear-fill', AuditLogs: 'bi-clipboard-data-fill', Database: 'bi-database-fill',
};

let activeRole = 'SuperAdmin';
let PERMISSIONS = [];
const rolePerms = {};

function fetchPermissionsForRole(roleName) {
    showLoader(true);

    return $.ajax({
        url: '/api/Get-Role-Permissions',
        method: 'GET',
        data: { roleName: roleName },
        dataType: 'json',
    })
        .done(function(response) {
            const grantedIds = new Set();

            $.each(response, function(i, item) {
                const perm = {
                    id: item.permissionId,
                    action: item.permissionName,
                    module: item.module,
                };

                const exists = $.grep(PERMISSIONS, function(x) { return x.id === perm.id; }).length > 0;
                if (!exists) PERMISSIONS.push(perm);

                if (item.isActive === true) {
                    grantedIds.add(item.permissionId);
                }
            });

            rolePerms[roleName] = grantedIds;
        })
        .fail(function(xhr, status, error) {
            showToast({ type: 'danger', title: 'Error', message: `Failed to load permissions for ${roleName}: ${error}` });
        })
        .always(function() {
            showLoader(false);
        });
}

function buildRolePills() {
    const $c = $('#rolePills').empty();
    $.each(ROLES, function(i, r) {
        const isActive = r.id === activeRole;
        const $btn = $('<button>', {
            class: 'btn btn-sm role-pill ' + (isActive ? 'btn-primary active' : 'btn-light-secondary'),
            html: `<i class="bi ${r.icon} me-1"></i>${r.label}`,
            click: function() { selectRole(r.id); }
        });
        $c.append($btn);
    });
}

function buildPermGrid() {
    const $grid = $('#permGrid').empty();
    const perms = rolePerms[activeRole] || new Set();

    const groups = {};
    $.each(PERMISSIONS, function(i, p) {
        if (!groups[p.module]) groups[p.module] = [];
        groups[p.module].push(p);
    });

    if ($.isEmptyObject(groups)) {
        $grid.html('<div class="text-muted text-center py-4">No permissions found for this role.</div>');
        updateStats();
        return;
    }

    $.each(groups, function(mod, items) {
        const ids = $.map(items, function(p) { return p.id; });
        const grantedCount = $.grep(items, function(p) { return perms.has(p.id); }).length;
        const icon = GROUP_ICONS[mod] || 'bi-circle';

        const $card = $('<div>', { class: 'card' });

        const $header = $('<div>', {
            class: 'group-header',
            html: `
                        <i class="bi ${icon} text-primary me-1"></i>
                        <span class="fw-semibold" style="font-size:13px;">${fmt(mod)}</span>
                        <span class="group-count" id="gc-${mod}">${grantedCount}/${items.length} granted</span>
                        <span class="toggle-all-link">
                            <i class="bi bi-toggles me-1"></i>Toggle All
                        </span>
                    `
        });

        $header.find('.toggle-all-link').on('click', function() {
            toggleGroup(mod, ids);
        });

        const $wrap = $('<div>', { class: 'perm-items-grid', id: `grp-${mod}` });

        $.each(items, function(i, p) {
            const granted = perms.has(p.id);

            const $row = $('<div>', {
                class: 'perm-row' + (granted ? ' granted' : ''),
                id: `pr-${p.id}`,
                html: `
                            <span class="perm-id">${p.id}</span>
                            <div class="flex-grow-1">
                                <div class="perm-action">${fmt(p.action)}</div>
                                <div class="perm-module">${fmt(mod)}</div>
                            </div>
                            <label class="perm-toggle">
                                <input type="checkbox" id="tog-${p.id}" ${granted ? 'checked' : ''}/>
                                <span class="slider"></span>
                            </label>
                        `
            });

            $row.on('click', function() {
                togglePerm(p.id, mod);
            });

            $row.find('.perm-toggle').on('click', function(e) {
                e.stopPropagation();
            });

            $row.find(`#tog-${p.id}`).on('change', function() {
                togglePerm(p.id, mod);
            });

            $wrap.append($row);
        });

        $card.append($header).append($wrap);
        $grid.append($card);
    });

    updateStats();
}

function selectRole(id) {
    activeRole = id;
    buildRolePills();

    const r = $.grep(ROLES, function(x) { return x.id === id; })[0];
    $('#currentRoleLabel').text(r.label);
    $('#roleBadge').text(r.label);

    if (rolePerms[id]) {
        buildPermGrid();
    } else {
        fetchPermissionsForRole(id).done(function() {
            buildPermGrid();
        });
    }
}

function togglePerm(id, mod) {
    const perms = rolePerms[activeRole];
    const $row = $(`#pr-${id}`);
    const $cb = $(`#tog-${id}`);

    if (perms.has(id)) {
        perms.delete(id);
        $row.removeClass('granted');
        $cb.prop('checked', false);
    } else {
        perms.add(id);
        $row.addClass('granted');
        $cb.prop('checked', true);
    }

    updateGroupCount(mod);
    updateStats();
}

function toggleGroup(mod, ids) {
    const perms = rolePerms[activeRole];
    const allGranted = ids.every(function(id) { return perms.has(id); });

    $.each(ids, function(i, id) {
        const $row = $(`#pr-${id}`);
        const $cb = $(`#tog-${id}`);

        if (allGranted) {
            perms.delete(id);
            $row.removeClass('granted');
            $cb.prop('checked', false);
        } else {
            perms.add(id);
            $row.addClass('granted');
            $cb.prop('checked', true);
        }
    });

    updateGroupCount(mod);
    updateStats();
}

function grantAll() {
    const r = $.grep(ROLES, function(x) { return x.id === activeRole; })[0];

    MzPopup.show({
        type: 'confirm',
        title: 'Grant All Permissions',
        message: `Do you want to grant all permissions to ${r.label}?`,
        okText: 'Yes',
        cancelText: 'Cancel',

        onOk: function() {
            const perms = rolePerms[activeRole];

            $.each(PERMISSIONS, function(i, p) {
                perms.add(p.id);
                $(`#pr-${p.id}`).addClass('granted');
                $(`#tog-${p.id}`).prop('checked', true);
            });

            refreshAllGroupCounts();
            updateStats();

            MzPopup.show({
                type: 'success',
                title: 'Updated',
                message: 'All permissions granted successfully.',
                okText: 'OK'
            });
        }
    });
}

function clearAll() {
    const r = $.grep(ROLES, function(x) { return x.id === activeRole; })[0];

    MzPopup.show({
        type: 'confirm',
        title: 'Clear All Permissions',
        message: `Do you want to remove all permissions for ${r.label}?`,
        okText: 'Yes',
        cancelText: 'Cancel',

        onOk: function() {
            const perms = rolePerms[activeRole];

            $.each(PERMISSIONS, function(i, p) {
                perms.delete(p.id);
                $(`#pr-${p.id}`).removeClass('granted');
                $(`#tog-${p.id}`).prop('checked', false);
            });

            refreshAllGroupCounts();
            updateStats();

            MzPopup.show({
                type: 'success',
                title: 'Cleared',
                message: 'All permissions removed successfully.',
                okText: 'OK'
            });
        }
    });
}

function savePermissions() {
    const r = $.grep(ROLES, function(x) { return x.id === activeRole; })[0];
    const grantedIds = [...rolePerms[activeRole]];

    MzPopup.show({
        type: 'confirm',
        title: 'Save Permissions',
        message: `Do you want to update permissions for ${r.label}?`,
        okText: 'Yes',
        cancelText: 'Cancel',

        onOk: function() {

            $.ajax({
                url: 'api/Permissions/save',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    roleName: activeRole,
                    permissionIds: grantedIds,
                }),
            })
                .done(function() {
                    MzPopup.show({
                        type: 'success',
                        title: 'Updated',
                        message: `Permissions for ${r.label} saved successfully.`,
                        okText: 'OK'
                    });
                })
                .fail(function(xhr, status, error) {
                    MzPopup.show({
                        type: 'danger',
                        title: 'Error',
                        message: `Save failed: ${error}`,
                        okText: 'OK'
                    });
                });

        }
    });
}

function updateStats() {
    const total = PERMISSIONS.length;
    const granted = (rolePerms[activeRole] || new Set()).size;
    $('#statGranted').text(granted);
    $('#statDenied').text(total - granted);
    $('#statTotal').text(total);
    $('#statCoverage').text(total ? Math.round(granted / total * 100) + '%' : '0%');
}

function updateGroupCount(mod) {
    const perms = rolePerms[activeRole] || new Set();
    const items = $.grep(PERMISSIONS, function(p) { return p.module === mod; });
    const g = $.grep(items, function(p) { return perms.has(p.id); }).length;
    $(`#gc-${mod}`).text(`${g}/${items.length} granted`);
}

function refreshAllGroupCounts() {
    const mods = [...new Set($.map(PERMISSIONS, function(p) { return p.module; }))];
    $.each(mods, function(i, m) { updateGroupCount(m); });
}

function showLoader(visible) {
    if (visible) {
        $('#permLoader').removeClass('d-none');
        $('#permGrid').addClass('opacity-50 pe-none');
    } else {
        $('#permLoader').addClass('d-none');
        $('#permGrid').removeClass('opacity-50 pe-none');
    }
}

let toastTimer;
function showToast(msg) {
   
    if (typeof msg === 'string') {
        const $toast = $('#toast');
        $('#toastMsg').html(msg);
        $toast.addClass('show');
        clearTimeout(toastTimer);
        toastTimer = setTimeout(function() {
            $toast.removeClass('show');
        }, 3000);
        return;
    }

    const {
        type = 'primary',
        title = '',
        message = '',
        icon,
        duration = 4000,
        variant,
        compact = false,
        actions = [],
    } = msg;

    var $container = $('#toast-container');
    if (!$container.length) return;

    var classes = ['mz-toast', 'toast-' + type];
    if (variant) classes.push(variant);
    if (compact) classes.push('compact');

    var iconClass = icon || ICON_MAP[type] || 'fa-circle-info';
    var timeStr = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    var actionsHTML = '';
    if (actions.length) {
        actionsHTML = '<div class="mz-toast-actions">' +
            actions.map(a =>
                `<button class="mz-toast-action-btn ${a.cls}">${a.label}</button>`
            ).join('') +
            '</div>';
    }

    var showMsg = message && !compact;
    var showTime = !compact && variant !== 'minimal';

    var toastHTML = `
    <div class="${classes.join(' ')}">
      <div class="mz-toast-inner">
        <div class="mz-toast-icon"><i class="fa-solid ${iconClass}"></i></div>
        <div class="mz-toast-body">
          <div class="mz-toast-title">${title}</div>
          ${showMsg ? `<div class="mz-toast-message">${message}</div>` : ''}
          ${compact && message ? `<div class="mz-toast-message">${message}</div>` : ''}
          ${showTime ? `<div class="mz-toast-time"><i class="fa-regular fa-clock"></i>${timeStr}</div>` : ''}
        </div>
        <button class="mz-toast-close">
          <i class="fa-solid fa-xmark"></i>
        </button>
      </div>
      ${actionsHTML}
      ${duration > 0 ? `<div class="mz-toast-progress">
        <div class="mz-toast-progress-bar" style="animation-duration:${duration}ms"></div>
      </div>` : ''}
    </div>
  `;

    var $toast = $(toastHTML);
    $container.append($toast);

    $toast.find('.mz-toast-close').on('click', function() {
        removeToast($toast);
    });

    $toast.find('.mz-toast-action-btn').each(function(i) {
        if (actions[i] && actions[i].onClick) {
            $(this).on('click', actions[i].onClick);
        }
        $(this).on('click', function() {
            removeToast($toast);
        });
    });

    if (duration > 0) {
        setTimeout(function() {
            removeToast($toast);
        }, duration);
    }
}

$(function() {
    if ($("#managePermissionsPage").length) {
        buildRolePills();
        selectRole('SuperAdmin');
    }
});




/******************************************** Logs Page ******************************************************************* */

var LogsPage = {

    state: {
        page: 1,
        size: 10,
        search: '',
        module: '',
        actionType: '',
        severity: '',
        date: ''
    },

    fetch: function() {
        $('#mzPageLoader').addClass('show');

        $.ajax({
            url: '/Logs',
            method: 'GET',
            data: LogsPage.state,

            success: function(response) {
                var $res = $(response);

                var newBody = $res.find('tbody').html();
                if (newBody) $('table tbody').html(newBody);

                var newLogsJson = $res.find('#logsJson').html();
                if (newLogsJson) {
                    logsData = JSON.parse(newLogsJson);
                }

                var $newFooter = $res.find('#tableFooter');
                if ($newFooter.length) {
                    $('#tableFooter')
                        .data('total-pages', $newFooter.data('total-pages'))
                        .data('current-page', $newFooter.data('current-page'))
                        .data('page-size', $newFooter.data('page-size'));
                }

                totalPages = parseInt($('#tableFooter').data('total-pages')) || 1;
                currentPage = LogsPage.state.page;
                pageSize = parseInt(LogsPage.state.size);

                renderPagination();
                LogsPage.updateResultsInfo();

                history.pushState(null, '', '/Logs?' + $.param(LogsPage.state));
            },

            error: function() {
                showToast({ type: 'danger', title: 'Error', message: 'Logs load karne mein error aaya.' });
            },

            complete: function() {
                $('#mzPageLoader').removeClass('show');
            }
        });
    },

    applyFilters: function() {
        LogsPage.state.search = $('input[name="search"]').val();
        LogsPage.state.module = $('select[name="module"]').val();
        LogsPage.state.actionType = $('select[name="actionType"]').val();
        LogsPage.state.severity = $('select[name="severity"]').val();
        LogsPage.state.date = $('input[name="date"]').val();
        LogsPage.state.page = 1;

        LogsPage.fetch();
    },

    bindRowsPerPage: function() {
        
    },

    updateResultsInfo: function() {
        var showing = $('table tbody tr').length;
        var tp = parseInt($('#tableFooter').data('total-pages')) || 1;
        var total = tp * parseInt(LogsPage.state.size);

        $('#resultsInfo').text('Showing ' + showing + ' of ' + total + ' logs');
    },

    resetFilters: function() {
        LogsPage.state = {
            page: 1,
            size: 10,
            search: '',
            module: '',
            actionType: '',
            severity: '',
            date: ''
        };

        $('input[name="search"]').val('');
        $('select[name="module"]').val('');
        $('select[name="actionType"]').val('');
        $('select[name="severity"]').val('');
        $('input[name="date"]').val('');

        LogsPage.fetch();
    },

    init: function() {

        if (!$('#logPage').length) return;

        LogsPage.state.page = parseInt($('#tableFooter').data('current-page')) || 1;
        LogsPage.state.size = parseInt($('#tableFooter').data('page-size')) || 10;

        LogsPage.updateResultsInfo();

        $(document).off('click.logsFilter').on('click.logsFilter', '#logPage button[type="submit"]', function(e) {
            e.preventDefault();
            LogsPage.applyFilters();
        });
    }
};

$(document).ready(function() {
    LogsPage.init();
});

$(document).ready(function() {

    $(document).off('change.rowsPerPage').on('change.rowsPerPage', '#rowsPerPage', function() {
        var newSize = $(this).val();
        
        console.log('Rows per page changed to:', newSize);

        var isUsersPage = $('#userTableBody').length > 0;
        var isLogsPage = $('#logPage').length > 0;
        var isBatchPage = $('#batchTableBody').length > 0;
        var isBatchPage = $('#batchTableBody').length > 0;
        var isMedicinePage = $('#medicineTableBody').length > 0;

        if (isUsersPage && !isLogsPage && !isBatchPage) {
            console.log('→ Loading USERS page data');
            UsersPage.state.size = newSize;
            UsersPage.state.page = 1;
            UsersPage.fetch();
            return;
        }

        if (isLogsPage && !isUsersPage && !isBatchPage) {
            console.log('→ Loading LOGS page data');
            LogsPage.state.size = newSize;
            LogsPage.state.page = 1;
            LogsPage.fetch();
            return;
        }

        if (isBatchPage && !isUsersPage && !isLogsPage) {
            console.log('→ Loading BATCH page data');
            BatchPage.state.size = newSize;
            BatchPage.state.page = 1;
            BatchPage.fetch();
            return;
        }

        if (isMedicinePage) {
            console.log('→ Loading BATCH page data');
            MedicinePage.state.size = newSize;
            MedicinePage.state.page = 1;
            MedicinePage.fetch();
            return;
        }
    });

});


$(document).off('click.pagination').on('click.pagination', '.pagination .page-link', function(e) {
    e.preventDefault();

    var $li = $(this).closest('.page-item');
    if ($li.hasClass('disabled') || $li.hasClass('active')) return;

    var p = parseInt($(this).attr('data-page'));
    if (!p || p < 1) return;

    var currentPath = window.location.pathname.toLowerCase();

    if (currentPath.indexOf('/users') !== -1 && $('#userTableBody').length) {
        UsersPage.state.page = p;
        UsersPage.fetch();
    } else if (currentPath.indexOf('/logs') !== -1 && $('#logPage').length) {
        LogsPage.state.page = p;
        LogsPage.fetch();
    } else if (currentPath.indexOf('/medicine/batch') !== -1 && $('#batchTableBody').length) {
        BatchPage.state.page = p;
        BatchPage.fetch();
    } else if (currentPath.indexOf('/medicine') !== -1 && $('#userTableBody').length) {
        // medicine list or other generic table pages — re-fetch via AJAX
        var url = window.location.pathname + '?page=' + p + '&size=' + pageSize;
        $('#mzPageLoader').addClass('show');
        $.ajax({
            url: url,
            method: 'GET',
            success: function(response) {
                var $res = $(response);
                var title = $res.filter('title').text() || $res.find('title').text();
                var content = $res.find('#mainContent').html();
                if (!content) content = response;

                $('#mainContent').html(content);
                history.pushState(null, title || document.title, url);
                if (title) { document.title = title; $('.top-bar-title').text(title); }

                if ($('#tableFooter').length) {
                    var $footer = $('#tableFooter');
                    totalPages = parseInt($footer.attr('data-total-pages')) || 1;
                    currentPage = parseInt($footer.attr('data-current-page')) || p;
                    pageSize = parseInt($footer.attr('data-page-size')) || 10;
                    renderPagination();
                }

                $('.col-toggle').each(function() {
                    if (!$(this).is(':checked')) $('.' + $(this).data('col')).hide();
                });
            },
            error: function() {
                window.location.href = url;
            },
            complete: function() {
                $('#mzPageLoader').removeClass('show');
            }
        });
    }
});

function esc(s) {
    return String(s ?? '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function di(label, val, full = false) {
    return `<div class="detail-item${full ? ' detail-full' : ''}"><dt>${label}</dt><dd>${val}</dd></div>`;
}

const ACTION_TYPE_MAP = {
    1: ['Create', 'success', 'bi-plus-circle'],
    2: ['Update', 'warning', 'bi-pencil'],
    3: ['Delete', 'danger', 'bi-trash']
};

const SEVERITY_MAP = {
    1: ['Info', 'info', 'bi-info-circle'],
    2: ['Warning', 'warning', 'bi-exclamation-triangle'],
    3: ['Error', 'danger', 'bi-x-octagon']
};

function atPill(t) {
    const [lbl, color, icon] = ACTION_TYPE_MAP[t] ?? ['Unknown', 'secondary', 'bi-question-circle'];
    return `<span class="badge bg-${color} bg-opacity-10 text-${color}" style="border:1px solid currentColor;">
                <i class="bi ${icon} me-1"></i>${lbl}
            </span>`;
}

function sevPill(s) {
    const [lbl, color, icon] = SEVERITY_MAP[s] ?? ['Unknown', 'secondary', 'bi-question'];
    return `<span class="badge bg-${color} bg-opacity-10 text-${color}" style="border:1px solid currentColor;">
                <i class="bi ${icon} me-1"></i>${lbl}
            </span>`;
}

function stBadge(st) {
    const color = st === 'Success' ? 'success' : st === 'Failed' ? 'danger' : 'secondary';
    const icon = st === 'Success' ? 'bi-check-circle' : st === 'Failed' ? 'bi-x-circle' : 'bi-dash-circle';
    return `<span class="badge bg-${color} bg-opacity-10 text-${color}" style="border:1px solid currentColor;">
                <i class="bi ${icon} me-1"></i>${esc(st)}
            </span>`;
}
function parseUA(ua) {
    if (!ua) return {
        browser: 'Unknown', browserVer: '', browserIcon: 'bi-globe',
        os: 'Unknown', osVer: '', device: 'Desktop', deviceIcon: 'bi-display'
    };

    let browser = 'Unknown', browserVer = '', browserIcon = 'bi-globe';
    const browsers = [
        { name: 'Edge', icon: 'bi-browser-edge', re: /Edg(?:e|)\/([\d.]+)/ },
        { name: 'Chrome', icon: 'bi-browser-chrome', re: /Chrome\/([\d.]+)/ },
        { name: 'Firefox', icon: 'bi-browser-firefox', re: /Firefox\/([\d.]+)/ },
        { name: 'Safari', icon: 'bi-browser-safari', re: /Version\/([\d.]+).*Safari/ },
        { name: 'Opera', icon: 'bi-browser-opera', re: /OPR\/([\d.]+)/ },
        { name: 'Samsung Browser', icon: 'bi-phone', re: /SamsungBrowser\/([\d.]+)/ },
    ];
    for (const b of browsers) {
        const m = ua.match(b.re);
        if (m) {
            browser = b.name;
            browserIcon = b.icon;
            browserVer = m[1].split('.')[0];
            break;
        }
    }

    let os = 'Unknown', osVer = '';
    const osList = [
        { name: 'Windows', re: /Windows NT ([\d.]+)/, ver: m => ({ '10.0': '10', '6.3': '8.1', '6.2': '8', '6.1': '7', '6.0': 'Vista', '5.1': 'XP' }[m[1]] ?? m[1]) },
        { name: 'macOS', re: /Mac OS X ([\d_]+)/, ver: m => m[1].replace(/_/g, '.') },
        { name: 'Android', re: /Android ([\d.]+)/, ver: m => m[1] },
        { name: 'iOS', re: /OS ([\d_]+) like Mac/, ver: m => m[1].replace(/_/g, '.') },
        { name: 'Linux', re: /Linux/, ver: () => '' },
        { name: 'ChromeOS', re: /CrOS/, ver: () => '' },
    ];
    for (const o of osList) {
        const m = ua.match(o.re);
        if (m) {
            os = o.name;
            osVer = o.ver(m);
            if (os === 'Windows' && osVer === '10') {
                const maj = (ua.match(/Chrome\/([\d]+)/) || [])[1];
                if (maj && parseInt(maj) >= 94) osVer = '10 / 11';
            }
            break;
        }
    }

    let device = 'Desktop', deviceIcon = 'bi-display';
    if (/Mobile|Android.*Mobile|iPhone|iPod/.test(ua)) { device = 'Mobile'; deviceIcon = 'bi-phone'; }
    else if (/iPad|Android(?!.*Mobile)|Tablet/.test(ua)) { device = 'Tablet'; deviceIcon = 'bi-tablet'; }

    return { browser, browserVer, browserIcon, os, osVer, device, deviceIcon };
}

function buildDeviceBlock(ua) {
    const p = parseUA(ua);
    return `
        <div class="device-info-grid mt-2">
            <div class="device-chip">
                <i class="bi ${p.deviceIcon} device-chip-icon"></i>
                <div>
                    <div class="device-chip-label">Device</div>
                    <div class="device-chip-val">${esc(p.device)}</div>
                </div>
            </div>
            <div class="device-chip">
                <i class="bi ${p.browserIcon} device-chip-icon"></i>
                <div>
                    <div class="device-chip-label">Browser</div>
                    <div class="device-chip-val">
                        ${esc(p.browser)}
                        ${p.browserVer ? `<span class="text-muted" style="font-size:10px;"> v${p.browserVer}</span>` : ''}
                    </div>
                </div>
            </div>
            <div class="device-chip">
                <i class="bi bi-pc-display-horizontal device-chip-icon"></i>
                <div>
                    <div class="device-chip-label">OS</div>
                    <div class="device-chip-val">
                        ${esc(p.os)}
                        ${p.osVer ? `<span class="text-muted" style="font-size:10px;"> ${p.osVer}</span>` : ''}
                    </div>
                </div>
            </div>
        </div>
        <div class="detail-item mt-2 detail-full">
            <dt>Raw UA</dt>
            <dd><code style="font-size:10px;word-break:break-all;color:var(--bs-secondary-color);">${esc(ua)}</code></dd>
        </div>`;
}

function fmtDate(dateString) {
    if (!dateString) return '-';

    const date = new Date(dateString);

    if (isNaN(date)) return '-';

    return date.toLocaleString('en-IN', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
}
function buildDrawerHTML(l) {
    return (
        `<div class="d-flex align-items-center flex-wrap gap-2 p-2 rounded mb-3"
              style="background:var(--bs-tertiary-bg);border:1px solid var(--bs-border-color);">`
        + atPill(l.actionType) + ' ' + sevPill(l.severity) + ' ' + stBadge(l.status)
        + `<span class="ms-auto text-muted" style="font-size:10.5px;">${fmtDate(l.createdAt)}</span>
         </div>`

        + `<p class="drawer-section-title"><i class="bi bi-info-circle"></i> Core Info</p>`
        + `<div class="detail-grid">`
        + di('Log ID', `<code style="font-size:11px;">#${l.logId}</code>`)
        + di('User', `<strong>${esc(l.userName ?? l.userId)}</strong><br>
                            <small class="text-muted">${esc(l.userId)} · ${esc(l.branchName ?? l.branchId ?? '—')}</small>`)
        + di('Module', `<strong>${esc(l.moduleName)}</strong>`)
        + di('Table', `<code style="font-size:11px;color:var(--bs-secondary-color);">${esc(l.tableName)}</code>`)
        + di('Record ID', esc(l.recordId))
        + di('Related ID', esc(l.relatedRecordId) || '—')
        + di('Session', `<code style="font-size:10.5px;">${esc(l.sessionId)}</code>`, true)
        + `</div>`

        + `<p class="drawer-section-title mt-3"><i class="bi bi-lightning-charge"></i> Action Detail</p>`
        + `<div class="detail-grid">`
        + di('Action', esc(l.action), true)
        + (l.changedFields
            ? di('Changed Fields',
                l.changedFields.split(',').map(f =>
                    `<span class="badge bg-primary bg-opacity-10 text-primary me-1" style="border:1px solid currentColor;">${esc(f.trim())}</span>`
                ).join(''), true)
            : '')
        + (l.delta
            ? (() => {
                try {
                    return di('Delta',
                        `<pre class="mb-0 text-success" style="font-size:11px;white-space:pre-wrap;">${esc(JSON.stringify(JSON.parse(l.delta), null, 2))}</pre>`,
                        true);
                } catch {
                    return di('Delta', `<code style="font-size:11px;">${esc(l.delta)}</code>`, true);
                }
            })()
            : '')
        + (l.notes ? di('Notes', esc(l.notes), true) : '')
        + `</div>`

        + (l.oldValue
            ? `<p class="drawer-section-title mt-3"><i class="bi bi-file-diff"></i> Value Diff</p>`
            + `<div class="diff-box">`
            + `<div class="diff-row"><span class="diff-sign old">−</span><span class="diff-content">${esc(l.oldValue)}</span></div>`
            + `<div class="diff-row"><span class="diff-sign new">+</span><span class="diff-content">${esc(l.newValue)}</span></div>`
            + `</div>`
            : '')

        + `<p class="drawer-section-title mt-3"><i class="bi bi-wifi"></i> Network & Device</p>`
        + `<div class="detail-grid">`
        + di('IP Address', `<code style="font-size:11px;">${esc(l.ipAddress)}</code>`)
        + di('Processed At', `<small>${fmtDate(l.processedAt)}</small>`)
        + `</div>`
        + buildDeviceBlock(l.deviceInfo)
    );
}

$(document).on("click", "tbody tr[data-log-id]", function() {
    const logId = $(this).data("log-id");

    $("#drawerLogId").text(`#${logId}`);
    $("#drawerBody").html(`
        <div class="d-flex justify-content-center align-items-center" style="height:180px;">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading…</span>
            </div>
        </div>`);
    $("#drawerOverlay, #logDrawer").addClass("open");

    $.getJSON(`/api/get-log-by-id?LogId=${logId}`)
        .done(function(res) {
            const l = res.logs?.[0];
            if (!l) {
                $("#drawerBody").html(`<div class="alert alert-warning m-3">No log data found.</div>`);
                return;
            }
            $("#drawerBody").html(buildDrawerHTML(l));

            $("#drExportBtn").off("click").on("click", function() {
                const blob = new Blob([JSON.stringify(l, null, 2)], { type: "application/json" });
                const a = Object.assign(document.createElement("a"), {
                    href: URL.createObjectURL(blob),
                    download: `log-${l.logId}.json`
                });
                a.click();
            });
        })
        .fail(function() {
            $("#drawerBody").html(`<div class="alert alert-danger m-3">Failed to load log details.</div>`);
        });
});

$(document).on("click", ".logdrawerclosebtn, #drawerOverlay", function() {
    $("#drawerOverlay, #logDrawer").removeClass("open");
});


/*********************************************** Medicine List **************************************************/

$(document).ready(function() {

    function di(label, val, full) {
        return '<div class="detail-item' + (full ? ' detail-full' : '') + '">'
            + '<dt>' + label + '</dt><dd>' + val + '</dd></div>';
    }

    function dic(icon, iconColor, iconBg, label, val, full) {
        return '<div class="detail-item' + (full ? ' detail-full' : '') + '" style="display:flex;align-items:flex-start;gap:10px;padding:9px 11px;">'
            + '<div style="width:30px;height:30px;border-radius:7px;background:' + iconBg + ';display:flex;align-items:center;justify-content:center;flex-shrink:0;margin-top:1px;">'
            + '<i class="bi ' + icon + '" style="color:' + iconColor + ';font-size:13px;"></i>'
            + '</div>'
            + '<div style="min-width:0;">'
            + '<dt style="font-size:10px;font-weight:700;text-transform:uppercase;letter-spacing:.5px;color:var(--bs-secondary-color);margin-bottom:3px;">' + label + '</dt>'
            + '<dd style="font-size:12.5px;font-weight:600;color:var(--bs-body-color);margin:0;word-break:break-word;">' + val + '</dd>'
            + '</div>'
            + '</div>';
    }

    $(document).on('click', '.med-drawer-close-btn, #medDrawerOverlay', function() {
        closeMedDrawer();
    });

    function openMedDrawer(id) {

        $('#medDrawerId').text('#' + id);

        $('#medDrawerBody').html(
            '<div class="text-center py-5">'
            + '<div class="spinner-border spinner-border-sm text-primary me-2"></div>'
            + '<span class="text-muted">Loading...</span>'
            + '</div>'
        );

        $('#medDrawerOverlay, #medDrawer').addClass('open');

        $.ajax({
            url: '/api/Medicine/Get?id=' + id,
            method: 'GET',
            success: function(res) {

                var medicine = res.records;

                $('#medDrawerId').text('#' + medicine.medicineId);

                $('#medExportBtn').off('click').on('click', function() {
                    exportMedJSON(medicine);
                });

                $('#medDrawerBody').html(

                    '<div class="d-flex align-items-center flex-wrap gap-2 p-2 rounded-3 mb-3"'
                    + ' style="background:var(--bs-tertiary-bg);border:1px solid var(--bs-border-color);">'

                    + (medicine.isActive
                        ? '<span class="badge bg-success-subtle text-success">Active</span>'
                        : '<span class="badge bg-secondary-subtle text-secondary">Inactive</span>')

                    + (medicine.isPrescriptionRequired
                        ? '<span class="badge bg-danger-subtle text-danger border border-danger-subtle">Rx Required</span>'
                        : '<span class="badge bg-success-subtle text-success border border-success-subtle">OTC</span>'
                    )

                    + (medicine.isDeleted
                        ? '<span class="badge">Deleted</span>'
                        : '')

                    + '<span class="ms-auto text-muted">'
                    + medicine.medicineId
                    + '</span>'
                    + '</div>'

                    + '<p class="drawer-section-title">Basic Info</p>'
                    + '<div class="detail-grid">'
                    + dic('bi-prescription2', '#0d6efd', 'rgba(13,110,253,.1)', 'Medicine Name', medicine.medicineName || '—', true)
                    + dic('bi-alphabet-uppercase', '#6f42c1', 'rgba(111,66,193,.1)', 'Generic Name', medicine.genericName || '—')
                    + dic('bi-tag-fill', '#0aa2c0', 'rgba(13,202,240,.1)', 'Category', medicine.category || '—')
                    + dic('bi-speedometer2', '#fd7e14', 'rgba(253,126,20,.1)', 'Strength', medicine.strength || '—')
                    + dic('bi-building-fill', '#6610f2', 'rgba(102,16,242,.1)', 'Manufacturer', medicine.manufacturer || '—', true)
                    + '</div>'

                    + '<p class="drawer-section-title mt-3">Tax & Stock</p>'
                    + '<div class="detail-grid">'
                    + dic('bi-upc-scan', '#198754', 'rgba(25,135,84,.1)', 'HSN Code', medicine.hsnCode || '—')
                    + dic('bi-percent', '#20c997', 'rgba(32,201,151,.1)', 'GST %', medicine.gstPercentage != null ? medicine.gstPercentage + '%' : '—')
                    + dic('bi-box-seam-fill', '#0d6efd', 'rgba(13,110,253,.1)', 'Min Stock Level', medicine.minimumStockLevel || '—')
                    + dic('bi-file-earmark-medical', '#ffc107', 'rgba(255,193,7,.1)', 'Prescription', medicine.isPrescriptionRequired ? 'Required' : 'Not Required')
                    + '</div>'

                    + '<p class="drawer-section-title mt-3">Audit Trail</p>'
                    + '<div class="detail-grid">'
                    + dic('bi-person-fill-add', '#6f42c1', 'rgba(111,66,193,.1)', 'Created By', medicine.createdBy || '—')
                    + dic('bi-calendar-plus-fill', '#0d6efd', 'rgba(13,110,253,.1)', 'Created At', fmtMedDate(medicine.createdAt))
                    + dic('bi-person-fill-gear', '#fd7e14', 'rgba(253,126,20,.1)', 'Updated By', medicine.updatedBy || '—')
                    + dic('bi-calendar-check-fill', '#20c997', 'rgba(32,201,151,.1)', 'Updated At', fmtMedDate(medicine.updatedAt))
                    + (medicine.isDeleted
                        ? dic('bi-person-fill-slash', '#dc3545', 'rgba(220,53,69,.1)', 'Deleted By', medicine.deletedBy || '—')
                        + dic('bi-calendar-x-fill', '#dc3545', 'rgba(220,53,69,.1)', 'Deleted At', fmtMedDate(medicine.deletedAt))
                        : '')
                    + '</div>'
                );
            },
            error: function() {
                $('#medDrawerBody').html(
                    '<div class="text-center py-5">'
                    + '<i class="bi bi-exclamation-circle fs-3 text-danger d-block mb-2"></i>'
                    + '<p class="text-muted">Failed to load medicine details.</p>'
                    + '</div>'
                );
            }
        });
    }

    $(document).on('click', 'tr.med-row', function(e) {

        if ($(e.target).closest('button, a').length) return;

        var id = $(this).attr('id');
        console.log('Medicine ID:', id);

        openMedDrawer(id);
    });

    $(document).on('click', '.medicineViewBtn', function() {
        const id = $(this).data('id');

        console.log('Medicine ID:', id);

        if (id) {
            openMedDrawer(id);
        } else {
            console.warn('No data-id found on clicked button');
        }
    });

    function closeMedDrawer() {
        $('#medDrawerOverlay, #medDrawer').removeClass('open');
    }

    function fmtMedDate(val) {
        if (!val) return '—';
        var d = new Date(val);
        if (isNaN(d)) return val;

        return d.toLocaleDateString('en-IN', {
            day: '2-digit',
            month: 'short',
            year: 'numeric'
        }) + ' ' + d.toLocaleTimeString('en-IN', {
            hour: '2-digit',
            minute: '2-digit',
            hour12: true
        });
    }

    function exportMedJSON(medicine) {
        var a = document.createElement('a');
        a.href = 'data:application/json;charset=utf-8,' +
            encodeURIComponent(JSON.stringify(medicine, null, 2));
        a.download = 'medicine_' + medicine.medicineId + '.json';
        a.click();
    }

});

var MedicinePage = {
        state: {
            page: 1, size: 10
        },

        fetch: function() {
            $('#mzPageLoader').addClass('show');
            $.ajax({
                url: '/Medicine/List',
                method: 'GET',
                data: MedicinePage.state,
                success: function(response) {
                    var $res = $(response);

                    var newBody = $res.find('#medicineTableBody').html();
                    if (newBody) $('#medicineTableBody').html(newBody);

                    var $newFooter = $res.find('#tableFooter');
                    if ($newFooter.length) {
                        $('#tableFooter')
                            .attr('data-total-pages', $newFooter.attr('data-total-pages'))
                            .attr('data-current-page', $newFooter.attr('data-current-page'))
                            .attr('data-page-size', $newFooter.attr('data-page-size'));
                    }

                    var newStats = $res.find('#statCards').html();
                    if (newStats) $('#statCards').html(newStats);

                    totalPages = parseInt($('#tableFooter').attr('data-total-pages')) || 1;
                    currentPage = MedicinePage.state.page;
                    pageSize = parseInt(MedicinePage.state.size);

                    renderPagination();
                    MedicinePage.updateResultsInfo();

                    history.pushState(null, '', '/Medicine/List?' + $.param(MedicinePage.state));
                },
                error: function() {
                    showToast({ type: 'danger', title: 'Error', message: 'Medicines load karne mein error aaya.' });
                },
                complete: function() {
                    $('#mzPageLoader').removeClass('show');
                }
            });
        },

        updateResultsInfo: function() {
            var showing = $('#medicineTableBody tr').length;
            var tp = parseInt($('#tableFooter').attr('data-total-pages')) || 1;
            var total = tp * parseInt(MedicinePage.state.size);
            $('#resultsInfo').text('Showing ' + showing + ' of ' + total + ' medicines');
        },

        resetFilters: function() {
            MedicinePage.state = { page: 1, size: 10 };
            MedicinePage.fetch();
        },

        init: function() {
            MedicinePage.state.page = parseInt($('#tableFooter').attr('data-current-page')) || 1;
            MedicinePage.state.size = parseInt($('#tableFooter').attr('data-page-size')) || 10;
            MedicinePage.updateResultsInfo();
        }
    };

/********************************************Batch List************************************ */

var BatchPage = {
    state: {
        page: 1, size: 10
    },

    fetch: function() {
        $('#mzPageLoader').addClass('show');
        $.ajax({
            url: '/Medicine/Batch/List',
            method: 'GET',
            data: BatchPage.state,
            success: function(response) {
                var $res = $(response);

                var newBody = $res.find('#batchTableBody').html();
                if (newBody) $('#batchTableBody').html(newBody);

                var $newFooter = $res.find('#tableFooter');
                if ($newFooter.length) {
                    $('#tableFooter')
                        .attr('data-total-pages', $newFooter.attr('data-total-pages'))
                        .attr('data-current-page', $newFooter.attr('data-current-page'))
                        .attr('data-page-size', $newFooter.attr('data-page-size'));
                }

                var newStats = $res.find('#statCards').html();
                if (newStats) $('#statCards').html(newStats);

                totalPages = parseInt($('#tableFooter').attr('data-total-pages')) || 1;
                currentPage = BatchPage.state.page;
                pageSize = parseInt(BatchPage.state.size);

                renderPagination();
                BatchPage.updateResultsInfo();

                history.pushState(null, '', '/Medicine/Batch/List?' + $.param(BatchPage.state));
            },
            error: function() {
                showToast({ type: 'danger', title: 'Error', message: 'Batches load karne mein error aaya.' });
            },
            complete: function() {
                $('#mzPageLoader').removeClass('show');
            }
        });
    },

    bindRowsPerPage: function() {
        // Now handled by delegated event handler at document level
    },

    updateResultsInfo: function() {
        var showing = $('#batchTableBody tr').length;
        var tp = parseInt($('#tableFooter').attr('data-total-pages')) || 1;
        var total = tp * parseInt(BatchPage.state.size);
        $('#resultsInfo').text('Showing ' + showing + ' of ' + total + ' batches');
    },

    resetFilters: function() {
        BatchPage.state = { page: 1, size: 10 };
        BatchPage.fetch();
    },

    init: function() {
        BatchPage.state.page = parseInt($('#tableFooter').attr('data-current-page')) || 1;
        BatchPage.state.size = parseInt($('#tableFooter').attr('data-page-size')) || 10;
        BatchPage.updateResultsInfo();
    }
};




var ExpiryPage = {
    state: {
        page: 1,
        size: 10,
        search: '',
        branch: '',
        category: '',
        sort: 'expDate_asc',
        window: '0',
        status: 'All'
    },

    fetch: function () {
        $('#mzPageLoader').addClass('show');
        $.ajax({
            url: '/Batch/Expiry/Report',
            method: 'GET',
            data: ExpiryPage.state,
            success: function (response) {
                var $res = $(response);

                var newBody = $res.find('#etTableBody').html();
                if (newBody) $('#etTableBody').html(newBody);

                var $newFooter = $res.find('#etTableFooter');
                if ($newFooter.length) {
                    $('#etTableFooter')
                        .attr('data-total-pages', $newFooter.attr('data-total-pages'))
                        .attr('data-current-page', $newFooter.attr('data-current-page'))
                        .attr('data-page-size', $newFooter.attr('data-page-size'));
                }

                var newStats = $res.find('#statCards').html();
                if (newStats) $('#statCards').html(newStats);

                ExpiryPage.renderPagination();
                ExpiryPage.updateResultsInfo();
                ExpiryPage.highlightActiveCard();
                ExpiryPage.reapplyColVisibility();

                history.pushState(null, '', '/Batch/Expiry/Report?' + $.param(ExpiryPage.state));
            },
            error: function () {
                showToast({ type: 'danger', title: 'Error', message: 'Expiry records load karne mein error aaya.' });
            },
            complete: function () {
                $('#mzPageLoader').removeClass('show');
            }
        });
    },
    reapplyColVisibility: function () {
        $('.et-col-toggle').each(function () {
            var cls = $(this).data('col');
            $('.' + cls).toggle(this.checked);
        });
    },

    renderPagination: function () {
        var totalPages = parseInt($('#etTableFooter').attr('data-total-pages')) || 1;
        var currentPage = parseInt($('#etTableFooter').attr('data-current-page')) || 1;
        var $ul = $('#etPagination').empty();

        $ul.append(
            '<li class="page-item ' + (currentPage <= 1 ? 'disabled' : '') + '">' +
            '<a class="page-link" href="#" data-page="' + (currentPage - 1) + '">«</a></li>'
        );

        var start = Math.max(1, currentPage - 2);
        var end = Math.min(totalPages, start + 4);
        if (end - start < 4) start = Math.max(1, end - 4);

        for (var i = start; i <= end; i++) {
            $ul.append(
                '<li class="page-item ' + (i === currentPage ? 'active' : '') + '">' +
                '<a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>'
            );
        }

        $ul.append(
            '<li class="page-item ' + (currentPage >= totalPages ? 'disabled' : '') + '">' +
            '<a class="page-link" href="#" data-page="' + (currentPage + 1) + '">»</a></li>'
        );
    },

    updateResultsInfo: function () {
        var showing = $('#etTableBody tr').length;
        var totalPages = parseInt($('#etTableFooter').attr('data-total-pages')) || 1;
        var size = parseInt(ExpiryPage.state.size) || 10;
        var currentPage = parseInt(ExpiryPage.state.page) || 1;

        var start = (currentPage - 1) * size + 1;
        var end = Math.min(currentPage * size, totalPages * size);

        $('#etResultsInfo').text('Showing ' + start + '–' + end + ' of ~' + (totalPages * size) + ' batches');
        $('#etResultCount').text(showing + ' record' + (showing !== 1 ? 's' : ''));
    },

    highlightActiveCard: function () {
        $('#statCards .card').removeClass('border-primary');
        var $card = $('#card-' + ExpiryPage.state.status);
        if ($card.length) $card.find('.card').addClass('border-primary');
    },

    filterByStatus: function (status) {
        ExpiryPage.state.status = status;
        ExpiryPage.state.page = 1;
        ExpiryPage.fetch();
    },

    applyFilters: function () {
        ExpiryPage.state.search = $('#etSearchInput').val().trim();
        ExpiryPage.state.branch = $('#etFilterBranch').val();
        ExpiryPage.state.category = $('#etFilterCategory').val();
        ExpiryPage.state.sort = $('#etSortField').val();
        ExpiryPage.state.window = $('#etWindowSelect').val();
        ExpiryPage.state.page = 1;
        ExpiryPage.fetch();
    },

    resetFilters: function () {
        $('#etSearchInput').val('');
        $('#etFilterBranch').val('');
        $('#etFilterCategory').val('');
        $('#etSortField').val('expDate_asc');
        $('#etWindowSelect').val('0');

        ExpiryPage.state = {
            page: 1, size: ExpiryPage.state.size,
            search: '', branch: '', category: '',
            sort: 'expDate_asc', window: '0', status: 'All'
        };

        ExpiryPage.fetch();
    },

    changePageSize: function (val) {
        ExpiryPage.state.size = parseInt(val) || 10;
        ExpiryPage.state.page = 1;
        ExpiryPage.fetch();
    },

    rebindRowEvents: function () {
        $('#etTableBody').off('click', '.et-view-btn').on('click', '.et-view-btn', function (e) {
            e.stopPropagation();
            ExpiryPage.openDrawer($(this).data('id'));
        });

        $('#etTableBody').off('click', '.et-row').on('click', '.et-row', function () {
            var id = $(this).find('.et-view-btn').data('id');
            if (id) ExpiryPage.openDrawer(id);
        });
    },

    openDrawer: function (batchId) {
        $('#expDrawerId').text('#' + batchId);
        $('#expDrawerBody').html(
            '<div class="text-center py-5">' +
            '<div class="spinner-border text-primary spinner-border-sm" role="status"></div>' +
            '<p class="text-muted mt-2 mb-0">Loading batch details...</p></div>'
        );
        $('#expDrawer').addClass('open');
        $('#expDrawerOverlay').addClass('active open');

        $.ajax({
            url: '/api/Batch/Report',
            method: 'GET',
            data: { id: batchId },
            success: function (data) {
                $('#expDrawerId').text('#' + (data.batchNumber || batchId));
                $('#expDrawerBody').html(ExpiryPage.drawerHtml(data));
                $('#expExportBtn').prop('hidden', false)
                    .off('click').on('click', function () {
                        ExpiryPage.exportDrawerJson(data);
                    });
            },
            error: function (xhr) {
                var msg = xhr.status === 404 ? 'Batch not found.' : 'Server error: ' + xhr.status;
                $('#expDrawerBody').html(
                    '<div class="alert alert-danger"><i class="bi bi-exclamation-circle me-2"></i>' +
                    '<strong>Error</strong><br>' + msg + '</div>'
                );
            }
        });
    },

    closeDrawer: function () {
        $('#expDrawer').removeClass('open');
        $('#expDrawerOverlay').removeClass('active open');
        $('#expExportBtn').prop('hidden', true);
    },

    exportExcel: function () {
        var headers = ['Medicine', 'Batch No', 'GRN', 'Mfg Date', 'Exp Date', 'Days Left', 'Stock', 'Status'];
        var rows = [];

        $('#etTableBody .et-row:visible').each(function () {
            var d = $(this).data();
            var daysLeft = parseInt(d.days);
            rows.push([
                d.name, d.batch, d.grn, d.mfgdate, d.expdate,
                daysLeft < 0 ? Math.abs(daysLeft) + 'd ago' : daysLeft + ' days',
                d.stock, d.status
            ]);
        });

        var csv = [headers].concat(rows)
            .map(function (r) { return r.map(function (v) { return '"' + v + '"'; }).join(','); })
            .join('\n');

        var a = document.createElement('a');
        a.href = 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv);
        a.download = 'expiry-tracker-' + new Date().toISOString().slice(0, 10) + '.csv';
        a.click();
    },

    exportDrawerJson: function (data) {
        var a = document.createElement('a');
        a.href = 'data:application/json;charset=utf-8,' + encodeURIComponent(JSON.stringify(data, null, 2));
        a.download = 'batch-' + (data.batchNumber || 'detail') + '.json';
        a.click();
    },

    initColToggles: function () {
        $(document).on('change', '.et-col-toggle', function () {
            var cls = $(this).data('col');
            $('.' + cls).toggle(this.checked);
        });

        $('.et-col-toggle').each(function () {
            if (!this.checked) $('.' + $(this).data('col')).hide();
        });
    },

    toggleSelectAll: function (master) {
        $('#etTableBody .et-row-check').prop('checked', master.checked);
    },

    drawerHtml: function (data) {
        const statusClass = data.status === "Expired" ? "danger" : data.status === "Soon" ? "warning" : "success";
        const daysLeft = data.daysLeft;
        const daysText = daysLeft < 0 ? `${Math.abs(daysLeft)} days ago` : `${daysLeft} days`;

        const di = (label, value, fullWidth = false) => `
            <div class="detail-item ${fullWidth ? 'full-width' : ''}">
              <span class="detail-label">${label}</span>
              <span class="detail-value">${value || "—"}</span>
            </div>
          `;

        const statusBadge = (text, variant) => `
            <span class="badge-pill badge-${variant}">${text}</span>
          `;

        const code = (text) => `<code class="code-block">${text || "—"}</code>`;

        const highlightBadge = (text, variant) => `
            <span class="badge bg-${variant} bg-opacity-10 text-${variant}" style="border: 1px solid currentColor;">
              ${text}
            </span>
          `;

        return `
            <style>
              .drawer-container {
                padding: 0;
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                color: var(--bs-body-color, #212529);
              }

              /* Header Pills */
              .drawer-header {
                display: flex;
                align-items: center;
                flex-wrap: wrap;
                gap: 0.5rem;
                padding: 1rem;
                background: var(--bs-tertiary-bg, #f8f9fa);
                border: 1px solid var(--bs-border-color, #dee2e6);
                border-radius: 0.375rem;
                margin-bottom: 1rem;
              }

              .badge-pill {
                display: inline-block;
                padding: 0.35rem 0.75rem;
                border-radius: 50px;
                font-size: 0.8rem;
                font-weight: 600;
                border: 1px solid;
                white-space: nowrap;
              }

              .badge-success {
                background: #d4edda;
                color: #155724;
                border-color: #c3e6cb;
              }

              .badge-warning {
                background: #fff3cd;
                color: #856404;
                border-color: #ffeaa7;
              }

              .badge-danger {
                background: #f8d7da;
                color: #721c24;
                border-color: #f5c6cb;
              }

              /* Dark mode badge adjustments */
              @media (prefers-color-scheme: dark) {
                .badge-success {
                  background: rgba(25, 135, 84, 0.25);
                  color: #75b798;
                  border-color: rgba(25, 135, 84, 0.5);
                }

                .badge-warning {
                  background: rgba(255, 193, 7, 0.25);
                  color: #ffc107;
                  border-color: rgba(255, 193, 7, 0.5);
                }

                .badge-danger {
                  background: rgba(220, 53, 69, 0.25);
                  color: #ea868f;
                  border-color: rgba(220, 53, 69, 0.5);
                }
              }

              .drawer-header-date {
                margin-left: auto;
                color: var(--bs-secondary-color, #6c757d);
                font-size: 0.75rem;
                white-space: nowrap;
              }

              /* Section Title */
              .drawer-section-title {
                margin: 1.5rem 0 0.75rem 0;
                padding: 0;
                font-size: 0.8rem;
                font-weight: 700;
                color: var(--bs-secondary-color, #495057);
                text-transform: uppercase;
                letter-spacing: 0.5px;
                display: flex;
                align-items: center;
                gap: 0.5rem;
              }

              .drawer-section-title i {
                font-size: 1rem;
                opacity: 0.7;
              }

              /* Detail Grid */
              .detail-grid {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                gap: 1rem 2rem;
                margin-bottom: 1rem;
              }

              .detail-item {
                display: flex;
                flex-direction: column;
                gap: 0.25rem;
              }

              .detail-item.full-width {
                grid-column: 1 / -1;
              }

              .detail-label {
                font-size: 0.8rem;
                color: var(--bs-secondary-color, #6c757d);
                font-weight: 500;
              }

              .detail-value {
                font-size: 0.95rem;
                color: var(--bs-body-color, #212529);
                line-height: 1.4;
              }

              .detail-value strong {
                font-weight: 700;
                color: var(--bs-body-color, #212529);
              }

              .detail-value small {
                font-size: 0.85rem;
                color: var(--bs-secondary-color, #6c757d);
              }

              /* Code Block */
              .code-block {
                background: var(--bs-secondary-bg, #f8f9fa);
                padding: 0.35rem 0.6rem;
                border-radius: 4px;
                font-size: 0.85rem;
                border: 1px solid var(--bs-border-color, #e9ecef);
                color: #d63031;
                font-weight: 500;
                font-family: 'Courier New', monospace;
              }

              @media (prefers-color-scheme: dark) {
                .code-block {
                  color: #ff6b6b;
                }
              }

              /* Diff Box */
              .diff-box {
                background: var(--bs-secondary-bg, #f8f9fa);
                border: 1px solid var(--bs-border-color, #e9ecef);
                border-radius: 6px;
                overflow: hidden;
                margin-bottom: 1rem;
              }

              .diff-row {
                display: flex;
                padding: 0.75rem;
                border-bottom: 1px solid var(--bs-border-color, #e9ecef);
                font-size: 0.9rem;
                align-items: flex-start;
                gap: 0.75rem;
              }

              .diff-row:last-child {
                border-bottom: none;
              }

              .diff-sign {
                flex-shrink: 0;
                font-weight: 700;
                font-size: 1rem;
                width: 20px;
                text-align: center;
              }

              .diff-sign.old {
                color: #dc3545;
              }

              .diff-sign.new {
                color: #198754;
              }

              @media (prefers-color-scheme: dark) {
                .diff-sign.old {
                  color: #ea868f;
                }

                .diff-sign.new {
                  color: #75b798;
                }
              }

              .diff-content {
                word-break: break-word;
                white-space: pre-wrap;
                color: var(--bs-body-color, #212529);
                font-family: 'Courier New', monospace;
                font-size: 0.85rem;
              }

              /* Pre/JSON Block */
              pre {
                background: var(--bs-secondary-bg, #f8f9fa);
                padding: 0.75rem;
                border-radius: 4px;
                border: 1px solid var(--bs-border-color, #e9ecef);
                font-size: 0.8rem;
                color: #198754;
                font-family: 'Courier New', monospace;
                white-space: pre-wrap;
                word-wrap: break-word;
              }

              @media (prefers-color-scheme: dark) {
                pre {
                  color: #75b798;
                }
              }

              /* Highlight Info */
              .highlight-info {
                background: #fffbf0;
                border-left: 3px solid #ff9800;
              }

              @media (prefers-color-scheme: dark) {
                .highlight-info {
                  background: rgba(255, 152, 0, 0.1);
                  border-left-color: #ff9800;
                }
              }

              .highlight-info .detail-value {
                color: #d84315;
                font-weight: 600;
              }

              @media (prefers-color-scheme: dark) {
                .highlight-info .detail-value {
                  color: #ffb74d;
                }
              }

              /* Changed Fields Container */
              .changed-fields {
                display: flex;
                flex-wrap: wrap;
                gap: 0.5rem;
              }

              .changed-field-badge {
                display: inline-block;
                padding: 0.4rem 0.7rem;
                border-radius: 4px;
                font-size: 0.8rem;
                background: #cfe2ff;
                color: #084298;
                border: 1px solid #b6d4fe;
                font-weight: 500;
              }

              @media (prefers-color-scheme: dark) {
                .changed-field-badge {
                  background: rgba(13, 110, 253, 0.25);
                  color: #6ea8fe;
                  border-color: rgba(13, 110, 253, 0.5);
                }
              }
            </style>

            <div class="drawer-container">
              <!-- Header with Status Pills -->
              <div class="drawer-header">
                ${statusBadge(data.status || "—", statusClass)}
                <span class="drawer-header-date">${data.createdAt || "—"}</span>
              </div>

              <!-- Core Info Section -->
              <p class="drawer-section-title">
                <i class="bi bi-capsule"></i> Medicine Info
              </p>
              <div class="detail-grid">
                ${di('Medicine Name', `<strong>${data.medicineName || "—"}</strong>`)}
                ${di('Medicine ID', code(data.medicineId || "—"))}
                ${di('Generic Name', data.genericName || "—")}
                ${di('Strength', data.strength || "—")}
                ${di('Manufacturer', data.manufacturer || "—")}
              </div>

              <!-- Batch Info Section -->
              <p class="drawer-section-title">
                <i class="bi bi-box-seam"></i> Batch Information
              </p>
              <div class="detail-grid">
                ${di('Batch No.', code(data.batchNumber || "—"))}
                ${di('GRN', code(data.grnNumber || "—"))}
                ${di('Category', data.category || "—")}
                ${di('HSN Code', data.hsnCode || "—")}
              </div>

              <!-- Expiry Section -->
              <p class="drawer-section-title">
                <i class="bi bi-hourglass-split"></i> Expiry Details
              </p>
              <div class="detail-grid highlight-info">
                ${di('Mfg Date', data.mfgDate || "—")}
                ${di('Exp Date', `<strong>${data.expDate || "—"}</strong>`)}
                ${di('Days Left', highlightBadge(daysText, statusClass))}
              </div>

              <!-- Stock & Pricing Section -->
              <p class="drawer-section-title">
                <i class="bi bi-currency-rupee"></i> Stock & Pricing
              </p>
              <div class="detail-grid">
                ${di('Stock Quantity', `<strong>${parseInt(data.totalStockReceived || 0).toLocaleString()}</strong>`)}
                ${di('Purchase Price', `₹${parseFloat(data.unitPurchasePrice || 0).toFixed(2)}`)}
                ${di('Selling Price', `₹${parseFloat(data.unitSellingPrice || 0).toFixed(2)}`)}
                ${di('GST %', `${data.gstPercentage || "—"}%`)}
                ${di('Rx Required', highlightBadge(data.isPrescriptionRequired ? 'Yes' : 'No', data.isPrescriptionRequired ? 'warning' : 'success'))}
              </div>

              <!-- Audit Info Section -->
              <p class="drawer-section-title">
                <i class="bi bi-clock-history"></i> Audit Information
              </p>
              <div class="detail-grid">
                ${di('Created By', data.createdBy || "—")}
                ${di('Created At', data.createdAt || "—")}
              </div>
            </div>
          `;
    },

    init: function () {
        ExpiryPage.state.page = parseInt($('#etTableFooter').attr('data-current-page')) || 1;
        ExpiryPage.state.size = parseInt($('#etTableFooter').attr('data-page-size')) || 10;

        ExpiryPage.renderPagination();
        ExpiryPage.updateResultsInfo();
        ExpiryPage.initColToggles();

        $(document).on('click', '#etTableBody .et-view-btn', function (e) {
            e.stopPropagation();
            ExpiryPage.openDrawer($(this).attr('data-id'));
        });

        $(document).on('click', '#etTableBody .et-row', function () {
            var id = $(this).find('.et-view-btn').attr('data-id');
            if (id) ExpiryPage.openDrawer(id);
        });

        $(document).on('click', '#etPagination .page-link', function (e) {
            e.preventDefault();
            var p = parseInt($(this).data('page'));
            var total = parseInt($('#etTableFooter').attr('data-total-pages')) || 1;
            if (!p || p < 1 || p > total) return;
            ExpiryPage.state.page = p;
            ExpiryPage.fetch();
        });

        $(document).on('change', '#etRowsPerPage', function () {
            ExpiryPage.changePageSize($(this).val());
        });

        var debounceTimer;
        $('#etSearchInput').on('input', function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(ExpiryPage.applyFilters, 350);
        });

        $('#etFilterBranch, #etFilterCategory, #etSortField, #etWindowSelect').on('change', ExpiryPage.applyFilters);
        $('#expDrawerOverlay, .exp-drawer-close-btn').on('click', ExpiryPage.closeDrawer);
        $('#etSelectAll').on('change', function () { ExpiryPage.toggleSelectAll(this); });
    }
};

function etFilterByStatus(s) { ExpiryPage.filterByStatus(s); }
function etApplyFilters() { ExpiryPage.applyFilters(); }
function etResetFilters() { ExpiryPage.resetFilters(); }
function etExportExcel() { ExpiryPage.exportExcel(); }
function etChangePageSize(v) { ExpiryPage.changePageSize(v); }
function etToggleSelectAll(el) { ExpiryPage.toggleSelectAll(el); }

$(document).ready(function () { ExpiryPage.init(); });