<template>
  <q-page class="flex flex-center">
    <div class="column items-center">
      

      <q-btn
        class="q-mt-md"
        color="primary"
        @click="nav"
        label="Go to Second Page"
        no-caps
      />
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function nav() {
  router.push({ path: '/second' })
}
</script>
