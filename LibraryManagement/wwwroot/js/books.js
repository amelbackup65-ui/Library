// Vanilla JavaScript for Book Management actions
document.addEventListener('DOMContentLoaded', () => {
    // باز کردن مودال تایید حذف و انتقال شناسه و عنوان کتاب به فرم مودال
    const deleteButtons = document.querySelectorAll('.delete-book-btn');
    const modalBookId = document.getElementById('modalBookId');
    const modalBookTitle = document.getElementById('modalBookTitle');

    deleteButtons.forEach(button => {
        button.addEventListener('click', () => {
            const bookId = button.getAttribute('data-book-id');
            const bookTitle = button.getAttribute('data-book-title');

            if (modalBookId && modalBookTitle) {
                modalBookId.value = bookId;
                modalBookTitle.textContent = `«${bookTitle}»`;
            }
        });
    });
});
