// Notification System using SignalR
(function() {
    'use strict';

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    let notificationCount = 0;

    connection.on("ReceiveNotification", function (notification) {
        console.log("Notification received:", notification);

        // Update badge
        notificationCount++;
        updateNotificationBadge();

        // Add to dropdown
        addNotificationToDropdown(notification);

        // Show toast notification
        showToastNotification(notification);
    });

    connection.start()
        .then(function () {
            console.log("Connected to notification hub");
            loadRecentNotifications();
        })
        .catch(function (err) {
            console.error("Error connecting to notification hub:", err);
            setTimeout(() => connection.start(), 5000);
        });

    function updateNotificationBadge() {
        const badge = document.getElementById('notificationBadge');
        if (notificationCount > 0) {
            badge.textContent = notificationCount > 99 ? '99+' : notificationCount;
            badge.style.display = 'inline-block';
        } else {
            badge.style.display = 'none';
        }
    }

    function addNotificationToDropdown(notification) {
        const list = document.getElementById('notificationList');
        const noNotifications = document.getElementById('noNotifications');

        if (noNotifications) {
            noNotifications.remove();
        }

        const item = document.createElement('li');
        item.innerHTML = `
            <a class="dropdown-item notification-item ${notification.type.toLowerCase()}" href="#" data-notification-id="${notification.id}">
                <div class="d-flex w-100 justify-content-between">
                    <strong class="mb-1">${escapeHtml(notification.title)}</strong>
                    <small class="text-muted">${formatTime(notification.createdAt)}</small>
                </div>
                <p class="mb-1 small">${escapeHtml(notification.message)}</p>
            </a>
        `;

        // Insert after header and divider
        const divider = list.querySelector('.dropdown-divider');
        if (divider && divider.nextSibling) {
            list.insertBefore(item, divider.nextSibling);
        } else {
            list.appendChild(item);
        }

        // Add click handler to mark as read
        item.querySelector('a').addEventListener('click', function(e) {
            e.preventDefault();
            markAsRead(notification.id);
            item.classList.add('read');
            notificationCount--;
            updateNotificationBadge();
        });
    }

    function showToastNotification(notification) {
        // Create toast container if it doesn't exist
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }

        const toast = document.createElement('div');
        toast.className = 'toast';
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        toast.innerHTML = `
            <div class="toast-header">
                <i class="bi bi-bell-fill me-2"></i>
                <strong class="me-auto">${escapeHtml(notification.title)}</strong>
                <small>${formatTime(notification.createdAt)}</small>
                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${escapeHtml(notification.message)}
            </div>
        `;

        toastContainer.appendChild(toast);
        const bsToast = new bootstrap.Toast(toast, { delay: 5000 });
        bsToast.show();

        toast.addEventListener('hidden.bs.toast', function() {
            toast.remove();
        });
    }

    async function loadRecentNotifications() {
        try {
            const response = await fetch('/api/notifications/recent');
            if (response.ok) {
                const notifications = await response.json();
                notifications.forEach(notification => {
                    if (!notification.isRead) {
                        addNotificationToDropdown(notification);
                        notificationCount++;
                    }
                });
                updateNotificationBadge();
            }
        } catch (err) {
            console.error("Error loading notifications:", err);
        }
    }

    async function markAsRead(notificationId) {
        try {
            await fetch(`/api/notifications/${notificationId}/read`, {
                method: 'POST'
            });
        } catch (err) {
            console.error("Error marking notification as read:", err);
        }
    }

    function formatTime(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diff = Math.floor((now - date) / 1000); // seconds

        if (diff < 60) return 'just now';
        if (diff < 3600) return Math.floor(diff / 60) + 'm ago';
        if (diff < 86400) return Math.floor(diff / 3600) + 'h ago';
        if (diff < 604800) return Math.floor(diff / 86400) + 'd ago';
        return date.toLocaleDateString();
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();
