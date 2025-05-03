window.elementHelper = {
    getBoundingClientRect: (element) => {
        if (!element) return null;
        const rect = element.getBoundingClientRect();
        return {
            x: rect.left,
            y: rect.top,
            width: rect.width,
            height: rect.height
        };
    }
};