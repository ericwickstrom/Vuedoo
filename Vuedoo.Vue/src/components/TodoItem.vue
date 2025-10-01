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
  <li class="group flex items-center gap-3 p-4 bg-white border-2 border-gray-200 rounded-lg hover:border-purple-300 hover:shadow-md transition-all">
    <input
      type="checkbox"
      :checked="todo.isComplete"
      @change="$emit('toggle', todo.id)"
      class="w-5 h-5 text-purple-600 border-gray-300 rounded focus:ring-purple-500 focus:ring-2 cursor-pointer"
    >
    <span
      class="flex-1 text-gray-800"
      :class="{ 'line-through text-gray-400': todo.isComplete }"
    >
      {{ todo.text }}
    </span>
    <span
      class="px-3 py-1 text-xs font-semibold rounded-full"
      :class="todo.isComplete ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'"
    >
      {{ todo.isComplete ? '✓ Done' : '⏱ Pending' }}
    </span>
    <button
      @click="$emit('delete', todo.id)"
      class="opacity-0 group-hover:opacity-100 text-red-500 hover:text-red-700 hover:bg-red-50 p-2 rounded-lg transition-all cursor-pointer"
      title="Delete todo"
    >
      🗑️
    </button>
  </li>
</template>
