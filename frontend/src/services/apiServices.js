import axios from 'axios'

const apiServices = {
  async get(url) {
    try {
      const response = await axios.get(url)
      return response.data
    } catch (error) {
      console.error('Error fetching data:', error)
      throw error
    }
  },
  async post(url, data) {
    try {
      const response = await axios.post(url, data)
      return response.data
    } catch (error) {
      console.error('Error creating data:', error)
      throw error
    }
  },
  async put(url, data) {
    try {
      const response = await axios.put(url, data)
      return response.data
    } catch (error) {
      console.error('Error updating data:', error)
      throw error
    }
  },
  async delete(url) {
    try {
      const response = await axios.delete(url)
      return response.data
    } catch (error) {
      console.error('Error deleting data:', error)
      throw error
    }
  },
}

export default apiServices
