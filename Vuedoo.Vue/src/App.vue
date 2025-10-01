<script setup lang="ts">
import TodoPage from './views/TodoPage.vue'
import ToastContainer from './components/ToastContainer.vue'
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-pink-50">
    <TodoPage />
    <ToastContainer />
  </div>
</template>

<style scoped>
</style>