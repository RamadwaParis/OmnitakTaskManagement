/**
 * Date Validation JavaScript
 * Provides client-side validation for due dates to prevent past dates
 */

document.addEventListener('DOMContentLoaded', function() {
    // Find all due date inputs
    const dueDateInputs = document.querySelectorAll('input[type="date"][name*="DueDate"], input[type="datetime-local"][name*="DueDate"]');
    
    dueDateInputs.forEach(function(input) {
        setupDateValidation(input);
    });
});

function setupDateValidation(input) {
    // Set minimum date to today
    const today = new Date();
    const todayString = today.toISOString().split('T')[0]; // Get YYYY-MM-DD format
    
    if (input.type === 'date') {
        input.min = todayString;
    } else if (input.type === 'datetime-local') {
        const todayTime = today.toISOString().slice(0, 16); // Get YYYY-MM-DDTHH:MM format
        input.min = todayTime;
    }

    // Add validation on change
    input.addEventListener('change', function() {
        validateDueDate(this);
    });

    // Add validation on blur
    input.addEventListener('blur', function() {
        validateDueDate(this);
    });

    // Initial validation if there's already a value
    if (input.value) {
        validateDueDate(input);
    }
}

function validateDueDate(input) {
    const selectedDate = new Date(input.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time for accurate comparison
    
    // Remove any existing error styling
    input.classList.remove('is-invalid');
    const existingError = input.parentElement.querySelector('.date-validation-error');
    if (existingError) {
        existingError.remove();
    }

    if (input.value && selectedDate < today) {
        // Add error styling
        input.classList.add('is-invalid');
        
        // Add error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback date-validation-error';
        errorDiv.textContent = 'Due date cannot be in the past. Please select today\'s date or a future date.';
        input.parentElement.appendChild(errorDiv);
        
        return false;
    }
    
    return true;
}

// Form submission validation
document.addEventListener('submit', function(e) {
    const form = e.target;
    const dueDateInputs = form.querySelectorAll('input[type="date"][name*="DueDate"], input[type="datetime-local"][name*="DueDate"]');
    let hasErrors = false;
    
    dueDateInputs.forEach(function(input) {
        if (!validateDueDate(input)) {
            hasErrors = true;
        }
    });
    
    if (hasErrors) {
        e.preventDefault();
        
        // Show notification
        showNotification('Please correct the due date errors before submitting.', 'error');
        
        // Focus on first invalid input
        const firstInvalid = form.querySelector('.is-invalid');
        if (firstInvalid) {
            firstInvalid.focus();
        }
    }
});

// Utility function to show notifications
function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show`;
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';
    
    notification.innerHTML = `
        <i class="fas fa-${type === 'error' ? 'exclamation-triangle' : 'info-circle'}"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    // Add to body
    document.body.appendChild(notification);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentElement) {
            notification.remove();
        }
    }, 5000);
}

// Add CSS for better styling
const style = document.createElement('style');
style.textContent = `
    .is-invalid {
        border-color: #dc3545 !important;
        box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
    }
    
    .invalid-feedback {
        display: block !important;
        width: 100%;
        margin-top: 0.25rem;
        font-size: 0.875em;
        color: #dc3545;
    }
    
    .date-validation-error {
        animation: fadeIn 0.3s ease-in;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-10px); }
        to { opacity: 1; transform: translateY(0); }
    }
`;
document.head.appendChild(style);
