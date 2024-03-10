// Define the function to animate the horizontal line
function animateHorizontalLine(targetWidth) {
    // Get the element for the horizontal line
    const horizontalLine = document.getElementById('horizontal-line');

    // Set the initial width (e.g., 0)
    let currentWidth = 0;
    // Set the target width you want to animate to (e.g., 80%)
    const targetWidthToshow = targetWidth *25;
    // Set the animation duration in milliseconds (e.g., 1000ms)
    const animationDuration = 1000;
    // Calculate the width increment per frame
    const widthIncrement = targetWidthToshow / (animationDuration / 16); // Assuming 60fps

    // Define the animation function
    function animate() {
        // Check if the current width is less than the target width
        if (currentWidth < targetWidthToshow) {
            // Increment the current width
            currentWidth += widthIncrement;

            // Set the width of the horizontal line
            horizontalLine.style.width = currentWidth + '%';

            // Request the next frame of animation
            requestAnimationFrame(animate);
        }
    }

    // Start the animation
    animate();
}
