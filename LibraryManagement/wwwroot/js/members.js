// Vanilla JavaScript for Member Management
document.addEventListener('DOMContentLoaded', () => {
    const deleteButtons = document.querySelectorAll('.delete-member-btn');
    const modalMemberId = document.getElementById('modalMemberId');
    const modalMemberName = document.getElementById('modalMemberName');

    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            modalMemberId.value = btn.getAttribute('data-member-id');
            modalMemberName.textContent = `«${btn.getAttribute('data-member-name')}»`;
        });
    });
});
