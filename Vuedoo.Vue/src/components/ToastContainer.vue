<script setup lang="ts">
import { useToast } from '@/composables/useToast'
import ToastNotification from '@/components/ToastNotification.vue'

const { toasts, removeToast } = useToast()
</script>

<template>
  <div class="fixed top-4 right-4 z-50 flex flex-col gap-3 pointer-events-none">
    <div
      v-for="toast in toasts"
      :key="toast.id"
      class="pointer-events-auto"
    >
      <ToastNotification
        :toast="toast"
        @close="removeToast"
      />
    </div>
  </div>
</template>
