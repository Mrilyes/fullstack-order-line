describe('WaitForApiCall', () => {
  it('Navigates to the Order Dashboard', () => {
    // Mock the API response
    cy.intercept('GET', 'https://localhost:7128/api/Order', {
      statusCode: 200,
    // Replace with mock data
    body: [
      {
        id: 1,
        customerName: 'IG',
        orderDate: '2023-10-01',
        orderNumber: 'ORD001111',
        "orderLines": [
      {
        "orderLineId": 2079,
        "orderId": 3,
        "articleId": 1001,
        "productName": "Product C",
        "quantity": 55,
        "price": 10
      },
    ]
      },
      {
        id: 2,
        customerName: 'Lebron',
        orderDate: '2023-10-02',
        orderNumber: 'ORD001222'
      },
      {
        id: 3,
        customerName: 'Curry',
        orderDate: '2024-12-02',
        orderNumber: 'ORD001333'
      }
    ]
    }).as('getOrders');

    cy.visit('/'); // Visit the default route

    // Wait for the API call to complete
    cy.wait('@getOrders');
  });
});
