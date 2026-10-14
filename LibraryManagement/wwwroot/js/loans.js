// Vanilla JavaScript for Loans & Returns management
document.addEventListener('DOMContentLoaded', () => {
    // باز کردن مودال تایید تحویل کتاب و انتقال مقادیر
    const returnButtons = document.querySelectorAll('.return-book-btn');
    const returnLoanId = document.getElementById('returnLoanId');
    const returnBookTitle = document.getElementById('returnBookTitle');
    const returnMemberName = document.getElementById('returnMemberName');

    returnButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const loanId = btn.getAttribute('data-loan-id');
            const bookTitle = btn.getAttribute('data-book-title');
            const memberName = btn.getAttribute('data-member-name');

            if (returnLoanId && returnBookTitle && returnMemberName) {
                returnLoanId.value = loanId;
                returnBookTitle.textContent = `«${bookTitle}»`;
                returnMemberName.textContent = `«${memberName}»`;
            }
        });
    });

    // باز کردن مودال تمدید امانت کتاب و انتقال مقادیر
    const extendButtons = document.querySelectorAll('.extend-book-btn');
    const extendLoanId = document.getElementById('extendLoanId');
    const extendBookTitle = document.getElementById('extendBookTitle');
    const extendMemberName = document.getElementById('extendMemberName');

    extendButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const loanId = btn.getAttribute('data-loan-id');
            const bookTitle = btn.getAttribute('data-book-title');
            const memberName = btn.getAttribute('data-member-name');

            if (extendLoanId && extendBookTitle && extendMemberName) {
                extendLoanId.value = loanId;
                extendBookTitle.textContent = `«${bookTitle}»`;
                extendMemberName.textContent = `«${memberName}»`;
            }
        });
    });
});
