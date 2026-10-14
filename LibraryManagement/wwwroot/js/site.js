// Vanilla JavaScript helper for alerts and interactive actions
document.addEventListener('DOMContentLoaded', () => {
    // محو شدن خودکار پیام‌های هشدار موفقیت پس از ۵ ثانیه
    const alerts = document.querySelectorAll('.alert-success');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 5000);
    });
});
