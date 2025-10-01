<script setup lang="ts">
import type { Toast } from '@/types/toast'

interface Props {
  toast: Toast
}

defineProps<Props>()
defineEmits<{
  close: [id: string]
}>()

const getToastStyles = (type: string) => {
  switch (type) {
    case 'success':
      return 'bg-gradient-to-r from-green-500 to-emerald-600 text-white'
    case 'error':
      return 'bg-gradient-to-r from-red-500 to-rose-600 text-white'
    case 'warning':
      return 'bg-gradient-to-r from-yellow-500 to-orange-500 text-white'
    case 'info':
      return 'bg-gradient-to-r from-blue-500 to-purple-600 text-white'
    default:
      return 'bg-gradient-to-r from-gray-500 to-gray-600 text-white'
  }
}

const getToastIcon = (type: string) => {
  switch (type) {
    case 'success':
      return '✓'
    case 'error':
      return '✗'
    case 'warning':
      return '⚠'
    case 'info':
      return 'ℹ'
    default:
      return '•'
  }
}
</script>

<template>
  <div
    class="flex items-center gap-3 px-4 py-3 rounded-lg shadow-lg min-w-[300px] max-w-md animate-slide-in"
    :class="getToastStyles(toast.type)"
  >
    <div class="flex-shrink-0 text-2xl">
      {{ getToastIcon(toast.type) }}
    </div>
    <div class="flex-1 font-medium">
      {{ toast.message }}
    </div>
    <button
      @click="$emit('close', toast.id)"
      class="flex-shrink-0 hover:bg-white/20 rounded p-1 transition-colors cursor-pointer"
      aria-label="Close notification"
    >
      ✕
    </button>
  </div>
</template>

<style scoped>
@keyframes slide-in {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.animate-slide-in {
  animation: slide-in 0.3s ease-out;
}
</style>
