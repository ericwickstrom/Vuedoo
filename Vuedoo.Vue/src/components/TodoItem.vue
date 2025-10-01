<script setup lang="ts">
import type { Todo } from '@/types/todo'

interface Props {
  todo: Todo
}

defineProps<Props>()
defineEmits<{
  toggle: [id: number]
  delete: [id: number]
}>()
</script>

<template>
  <li class="flex items-center justify-between">
    <div class="flex items-center">
      <input
        type="checkbox"
        :checked="todo.isComplete"
        @change="$emit('toggle', todo.id)"
        class="mr-2 cursor-pointer"
      >
      {{ todo.text }} - {{ todo.isComplete ? 'Done' : 'Pending' }}
    </div>
    <button @click="$emit('delete', todo.id)" class="cursor-pointer">❌</button>
  </li>
</template>
