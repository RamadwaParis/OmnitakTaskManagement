document.addEventListener('DOMContentLoaded', function() {
    // Get anti-forgery token
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const antiForgeryToken = tokenElement ? tokenElement.value : null;

    // Function to update notification badge
    function updateNotificationBadge() {
        fetch('/Notification/GetUnreadCount', {
            headers: {
                'RequestVerificationToken': antiForgeryToken
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                const badge = document.getElementById('notification-badge');
                if (badge) {
                    if (data.count > 0) {
                        badge.textContent = data.count;
                        badge.style.display = 'inline';
                    } else {
                        badge.style.display = 'none';
                    }
                }
            })
            .catch(error => {
                console.error('Error updating notifications:', error);
                // Optional: Implement retry logic here
            });
    }

    // Initial update
    updateNotificationBadge();

    // Update notifications every minute
    const updateInterval = setInterval(updateNotificationBadge, 60000);

    // Clean up interval when page is unloaded
    window.addEventListener('unload', function() {
        clearInterval(updateInterval);
    });

    // Update when the page becomes visible
    document.addEventListener('visibilitychange', function() {
        if (!document.hidden) {
            updateNotificationBadge();
        }
    });

    // Custom event for notification updates
    window.addEventListener('notificationUpdate', function() {
        updateNotificationBadge();
    });

    // Mark as read handler
    document.addEventListener('click', function(e) {
        const markReadBtn = e.target.closest('.mark-read-btn');
        if (markReadBtn && antiForgeryToken) {
            const notificationId = markReadBtn.dataset.id;
            
            fetch(`/Notification/MarkAsRead/${notificationId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': antiForgeryToken
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.success) {
                        updateNotificationBadge();
                        const notificationItem = markReadBtn.closest('.notification-item');
                        if (notificationItem) {
                            notificationItem.classList.remove('unread');
                            notificationItem.classList.add('read');
                            markReadBtn.style.display = 'none';
                        }
                    }
                })
                .catch(error => {
                    console.error('Error marking notification as read:', error);
                });
        }
    });
});