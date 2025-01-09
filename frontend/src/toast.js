import { useToast } from 'vue-toastification'

const toast = useToast()

export const showToast = (message, type = 'success') => {
  if (type === 'success') {
    toast.success(message)
  } else if (type === 'error') {
    toast.error(message)
  } else {
    toast.info(message)
  }
}
