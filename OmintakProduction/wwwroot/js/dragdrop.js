// Drag and Drop Functionality for Omnitak Task Management
class DragDropManager {
    constructor() {
        this.draggedElement = null;
        this.draggedData = null;
        this.init();
    }

    init() {
        this.initializeDraggableElements();
        this.initializeDropZones();
    }

    initializeDraggableElements() {
        const draggableElements = document.querySelectorAll('.draggable');
        
        draggableElements.forEach(element => {
            element.draggable = true;
            
            element.addEventListener('dragstart', (e) => {
                this.handleDragStart(e);
            });
            
            element.addEventListener('dragend', (e) => {
                this.handleDragEnd(e);
            });
        });
    }

    initializeDropZones() {
        const dropZones = document.querySelectorAll('.drop-zone');
        
        dropZones.forEach(zone => {
            zone.addEventListener('dragover', (e) => {
                this.handleDragOver(e);
            });
            
            zone.addEventListener('dragenter', (e) => {
                this.handleDragEnter(e);
            });
            
            zone.addEventListener('dragleave', (e) => {
                this.handleDragLeave(e);
            });
            
            zone.addEventListener('drop', (e) => {
                this.handleDrop(e);
            });
        });
    }

    handleDragStart(e) {
        this.draggedElement = e.target;
        this.draggedElement.classList.add('dragging');
        
        // Get data from the dragged element
        this.draggedData = {
            id: this.draggedElement.dataset.id,
            type: this.draggedElement.dataset.type, // 'ticket', 'task', 'project'
            status: this.draggedElement.dataset.status,
            priority: this.draggedElement.dataset.priority
        };
        
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/html', this.draggedElement.outerHTML);
        
        // Add visual feedback to drop zones
        this.highlightValidDropZones();
    }

    handleDragEnd(e) {
        if (this.draggedElement) {
            this.draggedElement.classList.remove('dragging');
        }
        this.draggedElement = null;
        this.draggedData = null;
        
        // Remove visual feedback from all drop zones
        this.removeDropZoneHighlights();
    }

    handleDragOver(e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
    }

    handleDragEnter(e) {
        e.preventDefault();
        e.currentTarget.classList.add('drag-over');
    }

    handleDragLeave(e) {
        // Only remove the class if we're actually leaving the drop zone
        if (!e.currentTarget.contains(e.relatedTarget)) {
            e.currentTarget.classList.remove('drag-over');
        }
    }

    handleDrop(e) {
        e.preventDefault();
        e.currentTarget.classList.remove('drag-over');
        
        if (!this.draggedElement || !this.draggedData) {
            return;
        }

        const dropZone = e.currentTarget;
        const newStatus = dropZone.dataset.status;
        const newPriority = dropZone.dataset.priority;
        
        // Only proceed if we're actually changing something
        if (newStatus === this.draggedData.status && newPriority === this.draggedData.priority) {
            return;
        }

        // Update the item
        this.updateItem(this.draggedData, newStatus, newPriority, dropZone);
    }

    highlightValidDropZones() {
        const dropZones = document.querySelectorAll('.drop-zone');
        dropZones.forEach(zone => {
            // You can add logic here to only highlight valid drop zones
            // based on the dragged item type
            zone.style.borderColor = 'var(--ms-blue)';
            zone.style.opacity = '1';
        });
    }

    removeDropZoneHighlights() {
        const dropZones = document.querySelectorAll('.drop-zone, .drag-over');
        dropZones.forEach(zone => {
            zone.classList.remove('drag-over');
            zone.style.borderColor = '';
            zone.style.opacity = '';
        });
    }

    updateItem(itemData, newStatus, newPriority, dropZone) {
        // Show loading indicator
        this.showLoadingIndicator(dropZone);
        
        // Prepare the update data
        const updateData = {
            id: itemData.id,
            type: itemData.type,
            newStatus: newStatus,
            newPriority: newPriority
        };

        // Make AJAX call to update the item
        fetch('/api/updateItemStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.getAntiForgeryToken(),
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify(updateData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Move the element to the new location
                this.moveElementToDropZone(this.draggedElement, dropZone);
                
                // Update the element's data attributes
                this.draggedElement.dataset.status = newStatus;
                if (newPriority) {
                    this.draggedElement.dataset.priority = newPriority;
                }
                
                // Show success message
                this.showNotification('Item updated successfully!', 'success');
            } else {
                this.showNotification('Failed to update item: ' + data.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error updating item:', error);
            this.showNotification('An error occurred while updating the item.', 'error');
        })
        .finally(() => {
            this.hideLoadingIndicator(dropZone);
        });
    }

    moveElementToDropZone(element, dropZone) {
        // Find the container within the drop zone where items should be placed
        const container = dropZone.querySelector('.kanban-items') || 
                         dropZone.querySelector('.items-container') || 
                         dropZone;
        
        // Add the element to the new location
        container.appendChild(element);
        
        // Add a small animation
        element.style.transform = 'scale(1.05)';
        setTimeout(() => {
            element.style.transform = '';
        }, 200);
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    showLoadingIndicator(element) {
        const loader = document.createElement('div');
        loader.className = 'ms-loading-overlay';
        loader.innerHTML = '<div class="ms-loading"></div>';
        element.appendChild(loader);
    }

    hideLoadingIndicator(element) {
        const loader = element.querySelector('.ms-loading-overlay');
        if (loader) {
            loader.remove();
        }
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `ms-notification ms-notification-${type}`;
        notification.innerHTML = `
            <div class="ms-notification-content">
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
                <span>${message}</span>
            </div>
            <button class="ms-notification-close" onclick="this.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        `;
        
        // Add to page
        document.body.appendChild(notification);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 5000);
    }

    // Method to refresh draggable elements after dynamic content is loaded
    refresh() {
        this.init();
    }

    // Method to make specific elements draggable
    makeDraggable(elements) {
        elements.forEach(element => {
            element.draggable = true;
            element.classList.add('draggable');
            
            element.addEventListener('dragstart', (e) => {
                this.handleDragStart(e);
            });
            
            element.addEventListener('dragend', (e) => {
                this.handleDragEnd(e);
            });
        });
    }
}

// Initialize drag and drop when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.dragDropManager = new DragDropManager();
});

// Utility function to refresh drag and drop after AJAX content loads
window.refreshDragDrop = function() {
    if (window.dragDropManager) {
        window.dragDropManager.refresh();
    }
};
