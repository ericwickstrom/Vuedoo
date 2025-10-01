<script setup lang="ts">
import { ref } from 'vue'
import TodoList from '@/components/TodoList.vue'
import type { Todo } from '@/types/todo'

const todos = ref<Todo[]>([
  { id: 1, text: "Learn Vue", isComplete: false },
  { id: 2, text: "Build todo app", isComplete: true },
  { id: 3, text: "Deploy to AWS", isComplete: false }
])

const newTodoText = ref('')

const addTodo = () => {
  if (newTodoText.value.trim()) {
    todos.value.push({
      id: Date.now(),
      text: newTodoText.value,
      isComplete: false
    })
    newTodoText.value = ''
  }
}

const toggleTodo = (id: number) => {
  const todo = todos.value.find(t => t.id === id)
  if (todo) {
    todo.isComplete = !todo.isComplete
  }
}

const deleteTodo = (id: number) => {
  todos.value = todos.value.filter(todo => todo.id !== id)
}
</script>

<template>
  <div>
    <h1>Vuedoo Todo App</h1>
    
    <form @submit.prevent="addTodo">
      <input v-model="newTodoText" placeholder="Enter new todo" />
      <button type="submit" class="cursor-pointer">➕</button>
    </form>

    <TodoList 
      :todos="todos"
      @toggle-todo="toggleTodo"
      @delete-todo="deleteTodo"
    />
  </div>
</template>