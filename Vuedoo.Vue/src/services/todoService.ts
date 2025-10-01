import type { Todo } from '@/types/todo'

interface CreateTodoRequest {
  text: string
}

interface UpdateTodoRequest {
  text?: string
  isComplete?: boolean
}

class TodoService {
  private baseUrl: string

  constructor(baseUrl: string = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/todos') {
    this.baseUrl = baseUrl
  }

  async getAllTodos(): Promise<Todo[]> {
    const response = await fetch(this.baseUrl)
    if (!response.ok) {
      throw new Error(`Failed to fetch todos: ${response.statusText}`)
    }
    return response.json()
  }

  async getTodoById(id: string): Promise<Todo> {
    const response = await fetch(`${this.baseUrl}/${id}`)
    if (!response.ok) {
      throw new Error(`Failed to fetch todo: ${response.statusText}`)
    }
    return response.json()
  }

  async createTodo(text: string): Promise<Todo> {
    const request: CreateTodoRequest = { text }
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })
    if (!response.ok) {
      throw new Error(`Failed to create todo: ${response.statusText}`)
    }
    return response.json()
  }

  async updateTodo(id: string, text?: string, isComplete?: boolean): Promise<Todo> {
    const request: UpdateTodoRequest = {}
    if (text !== undefined) request.text = text
    if (isComplete !== undefined) request.isComplete = isComplete

    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })
    if (!response.ok) {
      throw new Error(`Failed to update todo: ${response.statusText}`)
    }
    return response.json()
  }

  async deleteTodo(id: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'DELETE',
    })
    if (!response.ok) {
      throw new Error(`Failed to delete todo: ${response.statusText}`)
    }
  }
}

export const todoService = new TodoService()
export default todoService
