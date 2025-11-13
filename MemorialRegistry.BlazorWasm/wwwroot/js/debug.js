/* 
 * DEBUG UTILITIES FOR BLAZOR WEBASSEMBLY
 * =======================================
 * 
 * 🔧 MIGRATION: Added debug utilities for troubleshooting the migrated app
 * 
 * Features:
 * ✅ Keyboard shortcuts (Ctrl+D for debug toggle)
 * ✅ Console logging helpers
 * ✅ Performance monitoring
 * ✅ API connection testing utilities
 */

// 🎹 KEYBOARD SHORTCUTS: Global shortcut management
window.addKeyboardShortcut = (key, dotnetRef, methodName) => {
    document.addEventListener('keydown', (event) => {
        // Ctrl+D: Toggle debug panel
        if (event.ctrlKey && event.code === key) {
            event.preventDefault();
            dotnetRef.invokeMethodAsync(methodName);
            console.log('🐛 Debug panel toggled via keyboard shortcut');
        }
    });
};

// 📊 PERFORMANCE MONITORING: Track app performance
window.blazorDebug = {
    logApiCall: (url, method, duration) => {
        console.log(`🌐 API Call: ${method} ${url} (${duration}ms)`);
    },
    
    logComponentRender: (componentName, renderTime) => {
        console.log(`🎨 Component Rendered: ${componentName} (${renderTime}ms)`);
    },
    
    logNavigation: (from, to) => {
        console.log(`🧭 Navigation: ${from} → ${to}`);
    }
};

// 🏥 HEALTH CHECK: Monitor app health
window.checkAppHealth = () => {
    const health = {
        timestamp: new Date().toISOString(),
        browserSupport: {
            webAssembly: typeof WebAssembly !== 'undefined',
            localStorage: typeof Storage !== 'undefined',
            fetch: typeof fetch !== 'undefined'
        },
        performance: {
            memoryUsage: performance.memory ? performance.memory.usedJSHeapSize : 'N/A',
            timing: performance.timing ? performance.timing.loadEventEnd - performance.timing.navigationStart : 'N/A'
        }
    };
    
    console.table(health);
    return health;
};

console.log('🚀 Blazor WebAssembly Debug Utilities Loaded');
console.log('   Press Ctrl+D to toggle debug panel');
console.log('   Use window.checkAppHealth() for diagnostics');