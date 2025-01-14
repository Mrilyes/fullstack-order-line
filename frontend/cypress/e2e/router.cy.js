describe('Router', () => {
  it('Loads the OrderDashboardLayout component for the default route', () => {
    cy.visit('/'); // Visit the default route

    // Check if the Navbar exists
    cy.get('.navbar').should('exist');

    // Check if the left panel exists
    cy.get('.left-panel').should('exist');

    // Check if the middle panel exists
    cy.get('.middle-panel').should('exist');

    // Check if the right panel exists
    cy.get('.right-panel').should('exist');
  });
});
