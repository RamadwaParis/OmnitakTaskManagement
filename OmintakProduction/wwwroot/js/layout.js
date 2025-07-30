// Dropdown functionality
document.addEventListener('DOMContentLoaded', function() {
    // Handle dropdowns
    document.querySelectorAll('.dropdown-toggle').forEach(function(element) {
        element.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            const dropdown = this.closest('.dropdown');
            const menu = dropdown.querySelector('.dropdown-menu');
            
            // Close other dropdowns
            document.querySelectorAll('.dropdown.show').forEach(function(openDropdown) {
                if (openDropdown !== dropdown) {
                    openDropdown.classList.remove('show');
                    openDropdown.querySelector('.dropdown-menu').style.display = 'none';
                }
            });
            
            // Toggle current dropdown
            dropdown.classList.toggle('show');
            menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
        });
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
        if (!e.target.matches('.dropdown-toggle') && !e.target.closest('.dropdown-menu')) {
            document.querySelectorAll('.dropdown.show').forEach(function(dropdown) {
                dropdown.classList.remove('show');
                dropdown.querySelector('.dropdown-menu').style.display = 'none';
            });
        }
    });

    // Sidebar toggle functionality
    const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function() {
            document.body.classList.toggle('sidebar-collapsed');
            // Store the preference
            localStorage.setItem('sidebarCollapsed', document.body.classList.contains('sidebar-collapsed'));
        });

        // Restore sidebar state
        if (localStorage.getItem('sidebarCollapsed') === 'true') {
            document.body.classList.add('sidebar-collapsed');
        }
    }

    // Responsive navigation for mobile
    const mobileToggle = document.querySelector('.ms-header .ms-sidebar-toggle');
    if (mobileToggle) {
        mobileToggle.addEventListener('click', function() {
            document.body.classList.toggle('mobile-nav-open');
        });
    }

    // Close mobile nav when clicking outside
    document.addEventListener('click', function(e) {
        if (document.body.classList.contains('mobile-nav-open') &&
            !e.target.closest('.ms-sidebar') &&
            !e.target.closest('.ms-sidebar-toggle')) {
            document.body.classList.remove('mobile-nav-open');
        }
    });
});

// Collapsible sections
document.querySelectorAll('[data-toggle="collapse"]').forEach(function(trigger) {
    trigger.addEventListener('click', function(e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('data-target'));
        if (target) {
            target.classList.toggle('show');
        }
    });
});

// Alert dismissal
document.querySelectorAll('.alert .close').forEach(function(closeBtn) {
    closeBtn.addEventListener('click', function() {
        this.closest('.alert').remove();
    });
});
