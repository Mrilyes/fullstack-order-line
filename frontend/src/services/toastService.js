import { useToast } from 'vue-toastification'

const toastService = {
  success(message) {
    const toast = useToast()
    toast.success(message)
  },
  error(message) {
    const toast = useToast()
    toast.error(message)
  },
  warning(message) {
    const toast = useToast()
    toast.warning(message)
  },
}

export default toastService
