module Contexts 

open Feliz
open Types

module ModalContext=
            
    let createModalContext:Fable.React.IContext<DropdownModal> = React.createContext(name="modal") //makes context


