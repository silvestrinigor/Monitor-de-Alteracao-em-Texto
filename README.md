# Monitor de Alteração em Texto

Monitora alterações em um arquivo de texto e salva as modificações em um banco de dados SQLite.

![img1](https://github.com/silvestrinigor/Monitor-de-Alteracao-em-Texto/blob/master/Exemplo%20de%20uso.png)

## ✨ Sobre o Projeto

Este projeto faz parte de um **desafio técnico** para avaliação de habilidades em desenvolvimento de software. O objetivo é criar uma aplicação que monitora alterações em um arquivo `.txt`, registrando as modificações em um banco de dados SQLite a cada 30 segundos. O código deve seguir boas práticas de desenvolvimento, incluindo tratamento de erros, arquitetura bem definida e uso adequado de bibliotecas.

* * *

## 📝 Instalação

A versão mais recente pode ser baixada em [Releases](https://github.com/silvestrinigor/MonitorAlteracaoEmTexto/releases/tag/1.2).

### Requisitos

- Windows 10 ou superior
    
- .NET 8 Runtime instalado
    
- SQLite (embutido no projeto)
    

### Como Instalar

1.  Baixe o arquivo zip na página de lançamentos.
    
2.  Extraia os arquivos.
    
3.  Execute `setup.exe` para iniciar a instalação.

4.  Após a instalação, execute o programa pelo atalho criado no desktop (Se não funcionar, execute como Administrador).


* * *

## 📌 Dependências

Este projeto utiliza as seguintes dependências:

### ⚙️ Frameworks
- `Microsoft.NETCore.App`
- `Microsoft.WindowsDesktop.App.WindowsForms`

### 📦 Pacotes
- [`DiffPlex`](https://www.nuget.org/packages/DiffPlex/) (1.7.2) - Biblioteca para comparação de diferenças em texto.
- [`System.Data.SQLite`](https://www.nuget.org/packages/System.Data.SQLite/) (1.0.119) - Suporte para SQLite no .NET.


* * *

## 🌟 Licença

Este projeto está licenciado sob a [MIT License](https://github.com/silvestrinigor/Monitor-de-Alteracao-em-Texto/blob/master/LICENSE.txt).