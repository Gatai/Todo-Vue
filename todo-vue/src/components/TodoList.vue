<template>
  <div>
    <input type="text" class="todo-input"
      placeholder="what needs to be done" v-model="newTodo" @keyup.enter="addTodo" > 

        <transition-group name="fade" enter-active-class="animated fadeInUp" leave-active-class="animated fadeOutDown">  
            <div v-for="(todo, index) in todosFiltered" :key="todo.id" class="todo-item">
              <div class="todo-item-left">
                  
                  <input type="checkbox" v-model="todo.completed" @change="setToCompleted(index)" >
                  <div v-if="editingTodo !== todo" @dblclick="editTodo(todo)" class="todo-item-label" :class="{completed : todo.completed}" > 
                      {{todo.title}} 
                  </div>

                  <input v-else class="todo-item-edit" type="text" 
                        v-model="todo.title" @blur="doneEdit(todo)" 
                        @keyup.enter="doneEdit(todo)" @keyup.esc="cancelEdit(todo)" v-focus>  

              </div> 
              <div class="remove-item" @click="removeTodo(index)">
                  &times;   
              </div>
          </div>
      </transition-group>

      <div class="extra-container">
          <div><label><input type="checkbox" :checked="!anyRemaining" @change="checkAllTodos"> Check All</label></div>
          <div>{{ remaining }} items left</div>
      </div>

      <div class="extra-container">
      <div>
          <button :class="{ active: filter == 'all' }" @click="filter = 'all'">All</button>
          <button :class="{ active: filter == 'active' }" @click="filter = 'active'">Active</button>
          <button :class="{ active: filter == 'completed' }" @click="filter = 'completed'">Completed</button>
      </div>

      <div>
          <transition name="fade">
          <button v-if="showClearCompletedButton" @click="clearCompleted" >Clear completed</button>
          </transition>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'todo-list',
  data () {
    return{
        newTodo: '',
        editingTodo: null,
        idForTodo: 3,
        beforeEditCache: '',
        filter: 'all',
        todos: [
            {
                title: '',
                completed: false,
                editing: false,
            }
        ]
    }
  },
computed: {
  remaining(){
      return this.todos.filter(todo => !todo.completed).length
  },
  //För att få check all knappen att markeras när man har markerat klar för alla todos
  anyRemaining(){
      return this.remaining != 0;
  },
  todosFiltered(){
      if(this.filter == 'all'){
        return this.todos
      }else if (this.filter == 'active'){
        return this.todos.filter(todo => !todo.completed)
      }else if(this.filter == 'completed'){
        return this.todos.filter(todo => todo.completed)
      }
      return this.todos
  },
  showClearCompletedButton(){
      return this.todos.filter(todo => todo.completed).length > 0

  }
},
// När man skall editera en item i listan(todo) så vill man att symbolen för att kunna skriva synas efter att man har dubbel klickt 
 directives: {
    focus: {
      inserted: function (el) {
        el.focus()
      }
    }
  },
  methods: {
      addTodo(){
        if(this.newTodo.trim().length == 0){
            return
        }

        axios.post('https://localhost:44322/api/todo', {
          title: this.newTodo
        }).then(response =>{
          
          this.todos.push({
               id: response.data.id, 
               title: response.data.title,
               completed: response.data.completed,
           })
           
           this.newTodo = "";
        })
      },
      getTodos(){
         axios.get('https://localhost:44322/api/todo').then(response => {
           this.todos = response.data
         })
      },
      removeTodo(index){
           let id = this.todos[index].id
           axios.delete(`https://localhost:44322/api/todo/${id}`).then(
            this.todos.splice(index, 1)
         )
      },
      editTodo(todo){
        // this.beforeEditCache = todo.title
        todo.editing = true;
        this.editingTodo = todo;
      },
      doneEdit(todo){
          if(todo.title.trim() == ''){
              todo.title= this.beforeEditCache
          }
            todo.editing = false

            this.updateTitle(todo)
      },
    //   När man klickar på Esc så kan man ångra ändingen som gjordes.
      cancelEdit(todo){
          todo.title = this.beforeEditCache;
          todo.editing = false;
      },
      //Markera alla todo som "färdig"
      checkAllTodos(){
          this.todos.forEach((todo) => {
            todo.completed= event.target.checked
            axios.patch(`https://localhost:44322/api/todo/${todo.id}?completed=${todo.completed}`).then(
             )
            })
      },
      clearCompleted(){
        this.todos = this.todos.filter(todo => !todo.completed)
        axios.delete(`https://localhost:44322/api/todo/`)

     },
     setToCompleted(index){
       console.log('set to completed');
       let id = this.todos[index].id;
       let value = this.todos[index].completed;
       axios.patch(`https://localhost:44322/api/todo/${id}?completed=${value}`).then(
       )
     },
      updateTitle(todo){
       let id = todo.id
       let value = todo.title
       axios.patch(`https://localhost:44322/api/todo/${id}/title?title=${value}`).then(response => {
           this.todos = response.data
         }
       )
     }
  },
   mounted() {
          // Anropa backend API:et och hämta data
        this.getTodos();
      }
}
</script>

<style lang="scss">
@import url("https://cdnjs.cloudflare.com/ajax/libs/animate.css/3.5.2/animate.min.css");


.todo-input{
    width: 100%;
    padding: 10px 18px;
    font-size: 18px;
    margin-bottom: 16px;

     &:focus {
      outline: 0;
    }
}

.todo-item {
    margin-bottom: 12px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    animation-duration:0,3s;
}

.remove-item {
  cursor: pointer;
  margin-left: 14px;
  &:hover {
    color: black;
  }
}

.todo-item-left { // later
  display: flex;
  align-items: center;
}

.todo-item-label {
  padding: 10px;
  border: 1px solid white;
  margin-left: 12px;
}

.todo-item-edit {
  font-size: 24px;
  color: #2c3e50;
  margin-left: 12px;
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc; //override defaults
  font-family: 'Avenir', Helvetica, Arial, sans-serif;

  &:focus {
    outline: none;
  }
}

.completed {
  text-decoration: line-through;
  color: grey;
}

 .extra-container {
    display: flex;
    align-items: center;
    justify-content: space-between;
    font-size: 16px;
    border-top: 1px solid lightgrey;
    padding-top: 14px;
    margin-bottom: 14px;
  }

  button {
    font-size: 14px;
    background-color: white;
    appearance: none;
    &:hover {
      background: lightgreen;
    }

    &:focus {
      outline: none;
    }
  }

  .active {
    background: lightgreen;
  }

   .fade-enter-active, .fade-leave-active {
    transition: opacity .2s;
  }
  .fade-enter, .fade-leave-to {
    opacity: 0;
  }



</style>
