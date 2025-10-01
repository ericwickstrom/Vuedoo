<script setup lang="ts">
import TodoItem from '@/components/TodoItem.vue'
import type { Todo } from '@/types/todo'

interface Props {
  todos: Todo[]
}

defineProps<Props>()
defineEmits<{
  toggleTodo: [id: number]
  deleteTodo: [id: number]
}>()
</script>

<template>
  <ul class="space-y-3">
    <TodoItem
      v-for="todo in todos"
      :key="todo.id"
      :todo="todo"
      @toggle="$emit('toggleTodo', $event)"
      @delete="$emit('deleteTodo', $event)"
    />
  </ul>
</template>
