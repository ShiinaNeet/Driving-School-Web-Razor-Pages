// Chat System using SignalR
(function() {
    'use strict';

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    let unreadChatCount = 0;
    let currentChatUserId = null;
    let chatMessages = new Map(); // userId -> messages array

    connection.on("ReceiveMessage", function (data) {
        console.log("Message received:", data);

        const message = {
            id: data.id,
            senderId: data.senderId,
            senderName: data.senderName,
            message: data.message,
            sentAt: data.sentAt,
            isRead: false
        };

        // Add to messages map
        if (!chatMessages.has(data.senderId)) {
            chatMessages.set(data.senderId, []);
        }
        chatMessages.get(data.senderId).push(message);

        // If chat window is open and user is current chat
        if (currentChatUserId === data.senderId) {
            appendMessageToChat(message, false);
            markMessageAsRead(data.id);
        } else {
            // Update unread count
            unreadChatCount++;
            updateChatBadge();

            // Show notification
            showChatNotification(data.senderName, data.message);
        }
    });

    connection.on("MessageSent", function (data) {
        console.log("Message sent confirmation:", data);

        const message = {
            id: data.id,
            receiverId: data.receiverId,
            message: data.message,
            sentAt: data.sentAt,
            isSent: true
        };

        if (currentChatUserId === data.receiverId) {
            appendMessageToChat(message, true);
        }
    });

    connection.on("MessageRead", function (messageId) {
        // Update message status in UI if needed
        const messageElement = document.querySelector(`[data-message-id="${messageId}"]`);
        if (messageElement) {
            messageElement.classList.add('read');
        }
    });

    connection.on("UserOnline", function (userId) {
        updateUserOnlineStatus(userId, true);
    });

    connection.on("UserOffline", function (userId) {
        updateUserOnlineStatus(userId, false);
    });

    connection.start()
        .then(function () {
            console.log("Connected to chat hub");
        })
        .catch(function (err) {
            console.error("Error connecting to chat hub:", err);
            setTimeout(() => connection.start(), 5000);
        });

    // Chat icon click handler
    document.getElementById('chatIcon')?.addEventListener('click', function(e) {
        e.preventDefault();
        openChatWindow();
    });

    function openChatWindow() {
        // Check if chat modal already exists
        let chatModal = document.getElementById('chatModal');

        if (!chatModal) {
            createChatModal();
            chatModal = document.getElementById('chatModal');
        }

        const modal = new bootstrap.Modal(chatModal);
        modal.show();

        loadChatContacts();
    }

    function createChatModal() {
        const modalHtml = `
            <div class="modal fade" id="chatModal" tabindex="-1" aria-labelledby="chatModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="chatModalLabel">Messages</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body p-0">
                            <div class="row g-0" style="height: 500px;">
                                <div class="col-4 border-end">
                                    <div class="p-3 border-bottom">
                                        <input type="search" class="form-control form-control-sm" id="chatSearchInput" placeholder="Search contacts...">
                                    </div>
                                    <div id="chatContactsList" class="overflow-auto" style="height: calc(100% - 60px);">
                                        <div class="text-center p-3 text-muted">
                                            <div class="spinner-border spinner-border-sm" role="status">
                                                <span class="visually-hidden">Loading...</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-8">
                                    <div id="chatMessagesContainer" class="d-flex flex-column h-100">
                                        <div class="flex-grow-1 p-3 overflow-auto" id="chatMessages">
                                            <div class="text-center text-muted">Select a contact to start chatting</div>
                                        </div>
                                        <div class="p-3 border-top" id="chatInputContainer" style="display:none;">
                                            <div class="input-group">
                                                <input type="text" class="form-control" id="chatMessageInput" placeholder="Type a message..." disabled>
                                                <button class="btn btn-primary" type="button" id="chatSendBtn" disabled>
                                                    <i class="bi bi-send-fill"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', modalHtml);

        // Add event listeners
        document.getElementById('chatSendBtn').addEventListener('click', sendChatMessage);
        document.getElementById('chatMessageInput').addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                sendChatMessage();
            }
        });
    }

    async function loadChatContacts() {
        try {
            const response = await fetch('/api/chat/contacts');
            if (response.ok) {
                const contacts = await response.json();
                displayChatContacts(contacts);
            }
        } catch (err) {
            console.error("Error loading chat contacts:", err);
            document.getElementById('chatContactsList').innerHTML =
                '<div class="text-center p-3 text-danger">Error loading contacts</div>';
        }
    }

    function displayChatContacts(contacts) {
        const container = document.getElementById('chatContactsList');
        if (contacts.length === 0) {
            container.innerHTML = '<div class="text-center p-3 text-muted">No contacts available</div>';
            return;
        }

        let html = '';
        contacts.forEach(contact => {
            html += `
                <div class="chat-contact p-3 border-bottom" data-user-id="${contact.userId}" style="cursor: pointer;">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${escapeHtml(contact.name)}</strong>
                            <div class="small text-muted">${escapeHtml(contact.role)}</div>
                        </div>
                        <span class="online-status ${contact.isOnline ? 'online' : 'offline'}"></span>
                    </div>
                </div>
            `;
        });

        container.innerHTML = html;

        // Add click handlers
        container.querySelectorAll('.chat-contact').forEach(contact => {
            contact.addEventListener('click', function() {
                const userId = this.dataset.userId;
                openChat(userId, this.querySelector('strong').textContent);
            });
        });
    }

    async function openChat(userId, userName) {
        currentChatUserId = userId;

        // Update UI
        document.getElementById('chatInputContainer').style.display = 'block';
        document.getElementById('chatMessageInput').disabled = false;
        document.getElementById('chatSendBtn').disabled = false;
        document.getElementById('chatMessages').innerHTML = '';

        // Load message history
        try {
            const response = await fetch(`/api/chat/messages/${userId}`);
            if (response.ok) {
                const messages = await response.json();
                chatMessages.set(userId, messages);
                messages.forEach(msg => {
                    const isSent = msg.senderId !== userId;
                    appendMessageToChat(msg, isSent);
                });
            }
        } catch (err) {
            console.error("Error loading messages:", err);
        }
    }

    function appendMessageToChat(message, isSent) {
        const container = document.getElementById('chatMessages');
        const messageDiv = document.createElement('div');
        messageDiv.className = `message mb-2 ${isSent ? 'text-end' : ''}`;
        messageDiv.dataset.messageId = message.id;

        const time = formatTime(message.sentAt);

        messageDiv.innerHTML = `
            <div class="d-inline-block p-2 rounded ${isSent ? 'bg-primary text-white' : 'bg-light'}" style="max-width: 70%;">
                <div>${escapeHtml(message.message)}</div>
                <div class="small ${isSent ? 'text-white-50' : 'text-muted'}">${time}</div>
            </div>
        `;

        container.appendChild(messageDiv);
        container.scrollTop = container.scrollHeight;
    }

    async function sendChatMessage() {
        const input = document.getElementById('chatMessageInput');
        const message = input.value.trim();

        if (!message || !currentChatUserId) return;

        try {
            await connection.invoke("SendMessage", currentChatUserId, message);
            input.value = '';
        } catch (err) {
            console.error("Error sending message:", err);
            alert("Failed to send message. Please try again.");
        }
    }

    async function markMessageAsRead(messageId) {
        try {
            await connection.invoke("MarkAsRead", messageId);
        } catch (err) {
            console.error("Error marking message as read:", err);
        }
    }

    function updateChatBadge() {
        const badge = document.getElementById('unreadChatBadge');
        if (unreadChatCount > 0) {
            badge.textContent = unreadChatCount > 99 ? '99+' : unreadChatCount;
            badge.style.display = 'inline-block';
        } else {
            badge.style.display = 'none';
        }
    }

    function updateUserOnlineStatus(userId, isOnline) {
        const contact = document.querySelector(`.chat-contact[data-user-id="${userId}"] .online-status`);
        if (contact) {
            contact.classList.toggle('online', isOnline);
            contact.classList.toggle('offline', !isOnline);
        }
    }

    function showChatNotification(senderName, message) {
        // Reuse notification toast
        if (typeof showToastNotification !== 'undefined') {
            const notification = {
                title: `New message from ${senderName}`,
                message: message,
                createdAt: new Date().toISOString()
            };
            showToastNotification(notification);
        }
    }

    function formatTime(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diff = Math.floor((now - date) / 1000);

        if (diff < 60) return 'just now';
        if (diff < 3600) return Math.floor(diff / 60) + 'm ago';
        if (diff < 86400) return Math.floor(diff / 3600) + 'h ago';
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();
