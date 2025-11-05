# Notification and Chat System Guide

This document provides an overview of the newly implemented notification and real-time chat systems in the Driving School Enrollment System.

## Features Overview

### 1. Real-Time Notification System

The notification system provides instant alerts to users about important events in the enrollment system.

#### Notification Types

- **Enrollment Notifications**: Enrollment status changes, new enrollments, etc.
- **Payment Notifications**: Payment received, payment reminders, pending balances
- **Library Notifications**: Book due dates, overdue books, late fees
- **Course Notifications**: Course updates, capacity reached
- **System Notifications**: Account changes, system alerts

#### Who Gets Notifications?

1. **Students**:
   - Payment confirmations
   - Payment reminders
   - Enrollment status updates
   - Book due/overdue alerts

2. **Guardians**:
   - Student payment updates
   - Student enrollment changes
   - Payment reminders for their students

3. **Admins**:
   - Pending payment alerts (for balances > 7 days old)
   - New registrations
   - Payment confirmations

4. **Librarians**:
   - Book borrowing/return events
   - Overdue book alerts

5. **Professors**:
   - Course-related updates
   - Subject assignments

#### How to Use

- Click the **bell icon** in the navigation bar to view notifications
- Unread notifications show a red badge with count
- Click on a notification to mark it as read
- Recent notifications are loaded automatically on page load

### 2. Real-Time Chat System

The chat system enables direct communication between:
- **Staff** (Admin, Professor, Librarian) ↔ **Students/Guardians**
- Students and Guardians can chat with any staff member
- Staff members can chat with students and guardians

#### Features

- **Real-time messaging**: Messages appear instantly without page refresh
- **Online status**: See who's currently online (green dot)
- **Message history**: All conversations are saved
- **Unread indicators**: Badge shows number of unread messages
- **Read receipts**: Know when your messages have been read

#### How to Use

1. Click the **chat icon** (💬) in the navigation bar
2. Select a contact from the list (staff or students/guardians)
3. Type your message and press Enter or click Send
4. Messages appear in real-time
5. Green dot = user is online, Gray dot = offline

### 3. Background Job System (Hangfire)

A scheduled job runs daily at 9:00 AM to check for pending payments and send notifications.

#### Pending Payment Job

- Checks all enrollments with outstanding balances
- Sends alerts to admins for payments pending > 7 days
- Sends reminders to students and guardians every 7 days
- Automatic and requires no manual intervention

#### Accessing Hangfire Dashboard

- **URL**: `/hangfire` (only accessible to Admin role)
- View job status, execution history, and logs
- Manually trigger jobs if needed

## Technical Details

### Technologies Used

- **SignalR**: Real-time WebSocket communication
- **Hangfire**: Background job processing
- **Entity Framework Core**: Database operations
- **Bootstrap 5.3**: UI components
- **Bootstrap Icons**: Icons for notifications and chat

### Database Tables

Three new tables were added:

1. **Notifications**
   - Id, UserId, Title, Message, Type
   - IsRead, CreatedAt, ReadAt
   - RelatedEntityId, RelatedEntityType

2. **ChatMessages**
   - Id, SenderId, ReceiverId, Message
   - IsRead, SentAt, ReadAt, IsDeleted

3. **ChatConnections**
   - Id, UserId, ConnectionId
   - ConnectedAt, IsActive

### API Endpoints

#### Notifications
- `GET /api/notifications/recent` - Get recent notifications
- `POST /api/notifications/{id}/read` - Mark notification as read

#### Chat
- `GET /api/chat/contacts` - Get list of available contacts
- `GET /api/chat/messages/{userId}` - Get message history with a user

### SignalR Hubs

1. **NotificationHub** (`/notificationHub`)
   - Broadcasts real-time notifications to users
   - Handles notification read status

2. **ChatHub** (`/chatHub`)
   - Handles real-time messaging
   - Tracks online/offline status
   - Manages message delivery and read receipts

## Configuration

### Cron Job Schedule

The pending payment check runs on this schedule:
```
0 9 * * *  (Daily at 9:00 AM)
```

To change the schedule, edit `Program.cs`:
```csharp
RecurringJob.AddOrUpdate<PendingPaymentNotificationJob>(
    "check-pending-payments",
    job => job.CheckPendingPaymentsAsync(),
    "0 9 * * *"); // Modify this cron expression
```

### Cron Expression Examples

- `0 */6 * * *` - Every 6 hours
- `0 0 * * *` - Daily at midnight
- `0 9,17 * * *` - Daily at 9 AM and 5 PM
- `0 9 * * 1` - Every Monday at 9 AM

## Browser Compatibility

- Modern browsers with WebSocket support
- Chrome, Firefox, Safari, Edge (latest versions)
- SignalR will fallback to long-polling if WebSockets unavailable

## Troubleshooting

### Notifications not appearing?

1. Ensure you're logged in
2. Check browser console for errors
3. Verify SignalR connection status
4. Refresh the page to reconnect

### Chat not working?

1. Verify both users are logged in
2. Check if SignalR hub is running
3. Ensure firewall allows WebSocket connections
4. Check browser console for connection errors

### Background job not running?

1. Access `/hangfire` dashboard (Admin only)
2. Check if job is scheduled
3. View execution history for errors
4. Ensure Hangfire tables exist in database

## Future Enhancements

Potential improvements:

- Email notifications for important alerts
- SMS notifications for urgent messages
- Push notifications for mobile devices
- Group chat functionality
- File sharing in chat
- Voice/video calling
- Notification preferences/settings
- Mute/block functionality

## Support

For issues or questions, please contact the system administrator.
