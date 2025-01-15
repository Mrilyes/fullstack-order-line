describe('Router', () => {
  it('Navigates to the Order Dashboard', () => {
    cy.visit('/'); // Visit the default route
    cy.url().should('include', '/'); // Check if the URL is correct

  });

  it('Navigates to the Article Dashboard', () => {
    cy.visit('/articledashboard'); // Visit the Article Dashboard route
    cy.url().should('include', '/articledashboard'); // Check if the URL is correct
  });

  it('Navigates to the Home Page', () => {
    cy.visit('/homePage'); // Visit the Home Page route
    cy.url().should('include', '/homePage'); // Check if the URL is correct
  });
});
