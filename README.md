# Processador de Tarefas

**Sistema autônomo de execução de tarefas desenvolvido em .NET 8.0**

## 🎯 Objetivos

- O sistema vai permitir realizar as operações de criação, cancelamento e listagem de tarefas ativas (pendentes ou em execução).
    - Para tarefas inativas (concluídas ou canceladas), somente a listagem deve ser permitida.
- O processamento de uma quantidade determinada de tarefas deve acontecer em segundo plano.
- Ao ser criada, a tarefa deve atribuir a si mesmo um número aleatório de subtarefas entre 10 e 100.
- O processamento de cada sub-tarefa deve levar entre 3 e 60 segundos.
- O armazenamento em memória deve conter estaticamente uma lista de 100 tarefas para ser trabalhada a cada execução.

## 📌 Requisitos

- Injeção de dependência para repositórios e possível serviços.
- Execução de tarefas deve ser assíncrona.
- A implementação do repositório deve usar Generics.
- A máquina de estado de uma tarefa deve ser a seguinte:

- Permitir configurar:
    1. Quantidade de tarefas que podem ser executadas por vez.
    2. Quantidade máxima de subtarefas que cada tarefa pode receber.

