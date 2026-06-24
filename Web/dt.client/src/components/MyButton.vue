<template>
  <button class="my-btn" @click="$emit('click')">
    <slot />
  </button>
</template>

<script setup lang="ts">
defineEmits(['click'])
</script>

<style scoped>
.my-btn { padding: 8px 16px; }
</style>