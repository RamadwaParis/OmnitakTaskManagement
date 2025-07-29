// Task view management
document.addEventListener('DOMContentLoaded', function() {
    const boardView = document.getElementById('boardView');
    const listView = document.getElementById('listView');
    const viewToggleButtons = document.querySelectorAll('.task-filters .button');
    
    // Initial view setup
    initializeView();
    
    // View toggle functionality
    viewToggleButtons.forEach(button => {
        button.addEventListener('click', function() {
            const viewType = this.dataset.view;
            toggleView(viewType);
            saveViewPreference(viewType);
        });
    });
    
    // Task filtering functionality
    setupTaskFilters();
});

// Initialize view based on saved preference
function initializeView() {
    const savedView = localStorage.getItem('taskViewPreference') || 'board';
    toggleView(savedView);
}

// Toggle between board and list views
function toggleView(viewType) {
    const boardView = document.getElementById('boardView');
    const listView = document.getElementById('listView');
    const viewButtons = document.querySelectorAll('[data-view]');
    
    viewButtons.forEach(btn => btn.classList.remove('active'));
    document.querySelector(`[data-view="${viewType}"]`).classList.add('active');
    
    // Handle view transitions
    if (viewType === 'board') {
        listView.style.display = 'none';
        boardView.style.display = 'grid';
        setTimeout(() => boardView.classList.add('visible'), 10);
        listView.classList.remove('visible');
    } else {
        boardView.style.display = 'none';
        listView.style.display = 'block';
        setTimeout(() => listView.classList.add('visible'), 10);
        boardView.classList.remove('visible');
    }
}

// Save view preference
function saveViewPreference(viewType) {
    localStorage.setItem('taskViewPreference', viewType);
}

// Task filtering setup
function setupTaskFilters() {
    const filterButtons = document.querySelectorAll('.filter-group[data-filter-group="status"] .button');
    const tasksTable = document.getElementById('tasksTable');
    
    if (!tasksTable) return;
    
    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            const status = this.dataset.status;
            filterTasks(status);
            
            // Update active state
            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
        });
    });
}

// Filter tasks in list view
function filterTasks(status) {
    const rows = document.querySelectorAll('#tasksTable tbody tr');
    
    rows.forEach(row => {
        const rowStatus = row.dataset.status;
        const shouldShow = status === 'all' || 
                         (status === 'progress' && rowStatus === 'inprogress') ||
                         rowStatus === status;
        
        row.classList.toggle('hidden', !shouldShow);
    });
}
