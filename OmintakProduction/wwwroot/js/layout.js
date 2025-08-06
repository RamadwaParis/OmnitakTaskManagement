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

    // Enhanced Sidebar toggle functionality
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Add smooth transition
            const isCollapsed = document.body.classList.contains('sidebar-collapsed');
            
            if (!isCollapsed) {
                // Collapsing - add transition classes
                sidebar.style.overflow = 'hidden';
                document.body.classList.add('sidebar-collapsed');
                
                // Reset overflow after animation
                setTimeout(() => {
                    sidebar.style.overflow = 'auto';
                }, 400);
            } else {
                // Expanding
                document.body.classList.remove('sidebar-collapsed');
            }
            
            // Store the preference
            localStorage.setItem('sidebarCollapsed', document.body.classList.contains('sidebar-collapsed'));
            
            // Add visual feedback
            const icon = sidebarToggle.querySelector('i');
            if (icon) {
                icon.style.transform = 'scale(0.8)';
                setTimeout(() => {
                    icon.style.transform = '';
                }, 150);
            }
        });

        // Restore sidebar state on page load
        const savedState = localStorage.getItem('sidebarCollapsed');
        if (savedState === 'true') {
            document.body.classList.add('sidebar-collapsed');
        }
        
        // Add hover effects for better UX
        sidebar.addEventListener('mouseenter', function() {
            if (document.body.classList.contains('sidebar-collapsed')) {
                this.style.boxShadow = '4px 0 20px rgba(0, 0, 0, 0.1)';
            }
        });
        
        sidebar.addEventListener('mouseleave', function() {
            if (document.body.classList.contains('sidebar-collapsed')) {
                this.style.boxShadow = '0 2px 8px rgba(0, 0, 0, 0.06)';
            }
        });
    }

    // Responsive navigation for mobile
    const mobileToggle = document.getElementById('mobileMenuToggle');
    if (mobileToggle) {
        mobileToggle.addEventListener('click', function(e) {
            e.preventDefault();
            document.body.classList.toggle('mobile-nav-open');
        });
    }

    // Close mobile nav when clicking outside
    document.addEventListener('click', function(e) {
        if (document.body.classList.contains('mobile-nav-open') &&
            !e.target.closest('.ms-sidebar') &&
            !e.target.closest('.ms-mobile-menu-toggle')) {
            document.body.classList.remove('mobile-nav-open');
        }
    });
    
    // Handle window resize for responsive behavior
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            document.body.classList.remove('mobile-nav-open');
        }
    });
    
    // Active link highlighting
    const currentPath = window.location.pathname;
    const sidebarLinks = document.querySelectorAll('.ms-sidebar-link');
    
    sidebarLinks.forEach(link => {
        // Remove any existing active classes
        link.classList.remove('active');
        
        // Check if this link matches the current path
        const linkPath = link.getAttribute('href');
        if (linkPath && currentPath.includes(linkPath) && linkPath !== '/') {
            link.classList.add('active');
        } else if (currentPath === '/' && (linkPath === '/' || linkPath?.includes('Dashboard'))) {
            link.classList.add('active');
        }
    });
    
    // Smooth scroll to top on navigation
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function() {
            // Close mobile menu if open
            if (document.body.classList.contains('mobile-nav-open')) {
                document.body.classList.remove('mobile-nav-open');
            }
        });
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
    
    // Set active navigation link
    setActiveNavLink();
});

// Alert dismissal
document.querySelectorAll('.alert .close').forEach(function(closeBtn) {
    closeBtn.addEventListener('click', function() {
        this.closest('.alert').remove();
    });
});

// Set Active Navigation Link
function setActiveNavLink() {
    const currentPath = window.location.pathname.toLowerCase();
    const sidebarLinks = document.querySelectorAll('.ms-sidebar-link');
    
    sidebarLinks.forEach(link => {
        // Remove active class from all links
        link.classList.remove('active');
        
        // Get the href and normalize it
        const href = link.getAttribute('href');
        if (href) {
            const linkPath = href.toLowerCase();
            
            // Check for exact match or partial match
            if (currentPath === linkPath || 
                (linkPath !== '/' && currentPath.startsWith(linkPath)) ||
                isPathMatch(currentPath, linkPath)) {
                link.classList.add('active');
            }
        }
    });
}

// Enhanced path matching
function isPathMatch(currentPath, linkPath) {
    const currentSegments = currentPath.split('/').filter(s => s);
    const linkSegments = linkPath.split('/').filter(s => s);
    
    if (linkSegments.length === 0) return false;
    
    // Match controller
    if (currentSegments.length >= 1 && linkSegments.length >= 1) {
        return currentSegments[0] === linkSegments[0];
    }
    
    return false;
}
