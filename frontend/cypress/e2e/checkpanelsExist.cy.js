describe('CheckComponents', () => {
  it('Navigates to the Order Dashboard', () => {
    cy.visit('/'); // Visit the default route
    // cy.pause(); // Pause execution

    cy.url().should('include', '/'); // Check if the URL is correct

    // Check if the panels exist
    cy.get('.left-panel' , { timeout: 10000 }).should('exist'); // Check if the left panel exists
    cy.get('.middle-panel' , { timeout: 10000 }).should('exist'); // Check if the middle panel exists
    cy.get('.right-panel' , { timeout: 10000 }).should('exist'); // Check if the right panel exists
  });
});
