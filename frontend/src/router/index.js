import { createRouter, createWebHistory } from 'vue-router'

import ArticleDashboardLayout from '@/views/ArticleDashboardLayout.vue'
import OrderDashboardLayout from '@/views/OrderDashboardLayout.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: OrderDashboardLayout, // Load the DashboardLayout component
  },
  {
    path: '/articledashboard',
    name: 'articledashboard',
    component: ArticleDashboardLayout, // Load the DashboardLayout component
  },
  {
    path: '/orderdashboard',
    name: 'orderdashboard',
    component: OrderDashboardLayout, // Load the DashboardLayout component
  },


  {
    path: '/homePage',
    name: 'HomePage',
    component: () => import('@/components/HomePage.vue'), // Example lazy-loaded route
  },
  {
    path: '/dashboard',
    redirect: '/', // Redirect '/dashboard' to the home page
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/', // Catch-all route for unknown paths
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
