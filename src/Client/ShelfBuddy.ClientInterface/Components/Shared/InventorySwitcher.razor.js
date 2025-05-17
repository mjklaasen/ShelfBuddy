// inventorySwitcher.js - Handles click outside detection for the inventory switcher dropdown

// Create a namespace for our inventory switcher functions to avoid global namespace pollution
window.inventorySwitcher = {
    dotNetReference: null,

    // Initialize the click outside handler
    initialize: function (dotNetRef) {
        try {
            // Store the .NET reference
            this.dotNetReference = dotNetRef;

            // Remove any existing handler to prevent duplication
            document.removeEventListener("click", this.handleDocumentClick);

            // Add the click handler
            document.addEventListener("click", this.handleDocumentClick);

            console.log("Inventory switcher initialized successfully");
            return true;
        } catch (error) {
            console.error("Error initializing inventory switcher:", error);
            return false;
        }
    },

    // Handle document clicks
    handleDocumentClick: function (event) {
        try {
            // Check if we have a valid .NET reference
            if (!window.inventorySwitcher.dotNetReference) return;

            // Find all elements with the inventory-switcher class
            const switchers = document.querySelectorAll(".inventory-switcher");
            let isInside = false;

            // Check if the click was inside any inventory switcher
            for (let i = 0; i < switchers.length; i++) {
                if (switchers[i].contains(event.target)) {
                    isInside = true;
                    break;
                }
            }

            // If click was outside, notify the .NET component
            if (!isInside) {
                window.inventorySwitcher.dotNetReference.invokeMethodAsync("HandleClickOutside");
            }
        } catch (error) {
            console.error("Error handling click event:", error);
        }
    },

    // Clean up resources
    dispose: function () {
        try {
            // Remove the event listener
            document.removeEventListener("click", this.handleDocumentClick);

            // Clear the .NET reference
            this.dotNetReference = null;

            console.log("Inventory switcher disposed");
            return true;
        } catch (error) {
            console.error("Error disposing inventory switcher:", error);
            return false;
        }
    }
};

// Bind the document click handler to maintain the proper 'this' context
window.inventorySwitcher.handleDocumentClick = window.inventorySwitcher.handleDocumentClick.bind(window.inventorySwitcher);