<script setup lang="ts">
import { ref, computed } from 'vue'
import TodoList from '@/components/TodoList.vue'
import type { Todo } from '@/types/todo'
import { useToast } from '@/composables/useToast'

const { showToast } = useToast()

const todos = ref<Todo[]>([
  { id: '1', text: "Learn Vue", isComplete: false },
  { id: '2', text: "Build todo app", isComplete: true },
  { id: '3', text: "Deploy to AWS", isComplete: false }
])

const newTodoText = ref('')

const addTodo = () => {
  if (newTodoText.value.trim()) {
    todos.value.push({
      id: Date.now().toString(),
      text: newTodoText.value,
      isComplete: false
    })
    showToast('Todo added successfully! 🎉', 'success')
    newTodoText.value = ''
  } else {
    showToast('Please enter a todo', 'error')
  }
}

const toggleTodo = (id: string) => {
  const todo = todos.value.find(t => t.id === id)
  if (todo) {
    todo.isComplete = !todo.isComplete
    if (todo.isComplete) {
      showToast('Todo completed! ✓', 'success')
    } else {
      showToast('Todo marked as incomplete', 'warning')
    }
  }
}

const deleteTodo = (id: string) => {
  todos.value = todos.value.filter(todo => todo.id !== id)
  showToast('Todo deleted', 'info')
}

const incompleteTodos = computed(() => todos.value.filter(t => !t.isComplete).length)
const completedTodos = computed(() => todos.value.filter(t => t.isComplete).length)
</script>

<template>
  <div class="max-w-2xl mx-auto px-4 py-12">
    <div class="bg-white rounded-2xl shadow-2xl overflow-hidden">
      <!-- Header -->
      <div class="bg-gradient-to-r from-purple-600 to-blue-600 px-8 py-6">
        <h1 class="text-4xl font-bold text-white text-center">✨ Vuedoo</h1>
        <p class="text-purple-100 text-center mt-2">Your beautiful todo companion</p>
      </div>

      <!-- Add Todo Form -->
      <div class="p-6 bg-gray-50 border-b border-gray-200">
        <form @submit.prevent="addTodo" class="flex gap-3">
          <input
            v-model="newTodoText"
            placeholder="What needs to be done?"
            class="flex-1 px-4 py-3 border-2 border-gray-300 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-200 transition-all"
          />
          <button
            type="submit"
            class="px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg font-semibold hover:from-purple-700 hover:to-blue-700 transition-all shadow-md hover:shadow-lg cursor-pointer"
          >
            ➕ Add
          </button>
        </form>
      </div>

      <!-- Todo List -->
      <div class="p-6">
        <TodoList
          :todos="todos"
          @toggle-todo="toggleTodo"
          @delete-todo="deleteTodo"
        />

        <!-- Empty State -->
        <div v-if="todos.length === 0" class="text-center py-12">
          <div class="text-6xl mb-4">📝</div>
          <p class="text-gray-500 text-lg">No todos yet. Add one above!</p>
        </div>
      </div>

      <!-- Footer Stats -->
      <div v-if="todos.length > 0" class="px-8 py-4 bg-gray-50 border-t border-gray-200">
        <div class="flex justify-between text-sm text-gray-600">
          <span>{{ incompleteTodos }} remaining</span>
          <span>{{ completedTodos }} completed</span>
        </div>
      </div>
    </div>
  </div>
</template>