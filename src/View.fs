namespace App

open Feliz
open Feliz.Bulma

open Feliz
open Feliz.Bulma
open Browser.Dom
open Browser.Types
open Fable.SimpleJson
open Fable.Core.JS
open System

module Types =
  type Protocoltext = {
      Content: string list
  }

  type Annotation = {
      Key: string
  }

  type ModalInfo = {
      isActive: bool
      location: int * int
  }

  type DropdownModal = {
      modalState: ModalInfo
      setter: ModalInfo -> unit 
  }

  [<RequireQualifiedAccess>]

  type Page =
      |Builder
      |Contact
      |Help

open Types

module ModalContext=
    let createModalContext:Fable.React.IContext<DropdownModal> = React.createContext(name="modal") //makes context


// module ContextHelper =

//   let addAnnotation =
//       (fun (e:MouseEvent) ->
//           let term = window.getSelection().ToString().Trim()
//           if term.Length <> 0 then 
//               let newAnno = (AnnotationState) //errors are defined in Builder.fs
//               newAnno
//               |> fun t ->

//               t |> setAnnotationState
//               t |> setLocalStorageAnnotation "Annotations"
//           else 
//               ()
//           Browser.Dom.window.getSelection().removeAllRanges()
//       )

module ContextMenu =
        
    [<ReactComponent>]
    let private Menu (mousex: int, mousey: int) (resetter: unit-> unit) =
        let ele = React.useInputRef()
        React.useLayoutEffectOnce(fun _ -> 
          if ele.current.IsSome then ele.current.Value.focus() )
        Html.div [
            prop.ref ele
            prop.className "fixed z-20 block bg-slate-800 rounded shadow"
            prop.style [ style.top mousey; style.left mousex ]
            // prop.onClick ContextHelper.addAnnotation
            prop.custom ("focused", "true")
            prop.tabIndex 0
            prop.onBlur (fun _ -> resetter() )
            prop.children [
              Html.ul [
                for i in 0 .. 5 do
                  Html.li [
                    prop.className "py-1 px-2 hover:bg-slate-700 transition-colors"
                    prop.children [
                      Html.button [
                        prop.text $"Option {i}"
                        prop.onClick (fun _ -> 
                          Browser.Dom.console.log("Clicked Option " + i.ToString())
                          resetter() 
                        )
                      ] 
                    ]
                  ]
              ]
            ]
        ]

    let initialModal = {
        isActive = false
        location = (0,0)
    }

    let ContextMenu (modalContext:DropdownModal) = 
        let resetter = fun () -> modalContext.setter initialModal //add actual function
        // let rmv = modalContext.setter initialModal 
        Menu modalContext.modalState.location resetter

type Builder =
    [<ReactComponent>]

    static member Main() =
        let isLocalStorageClear (key:string) () =
            match (Browser.WebStorage.localStorage.getItem key) with
            | null -> true // Local storage is clear if the item doesn't exist
            | _ -> false //if false then something exists and the else case gets started

        let initialInteraction (id: string) =
            if isLocalStorageClear id () = true then []
            else Json.parseAs<Annotation list> (Browser.WebStorage.localStorage.getItem id)  

        let (AnnotationState: Annotation list, setAnnotationState) = React.useState (initialInteraction "Annotations")

        let setLocalStorageAnnotation (id: string)(nextAnnos: Annotation list) =
            let JSONString = Json.stringify nextAnnos 
            Browser.WebStorage.localStorage.setItem(id, JSONString)

        let modalContext = React.useContext (ModalContext.createModalContext)

        Html.div [
            Bulma.block [
                prop.onContextMenu (fun e ->
                    Browser.Dom.console.log("Right Click")
                    let term = window.getSelection().ToString().Trim() 
                    Browser.Dom.console.log(term)
                    // comment out the condition below for faster testing
                    // if term.Length <> 0 then 
                    modalContext.setter {
                        isActive = true;
                        location = int e.pageX, int e.pageY
                    }
                    // else 
                    //     ()
                    e.stopPropagation()
                    e.preventDefault()
                )
                prop.text "CLICK ME"
            ]
        ]


type View =
    [<ReactComponent>]
    static member Main() =

        let (modalState: ModalInfo, setModal) =
            React.useState(ContextMenu.initialModal)               
                       
        let myModalContext = { //makes setter and state in one record type
            modalState = modalState
            setter = setModal
            }

        let modalactivator = 
            match modalState.isActive with
            |true -> ContextMenu.ContextMenu (myModalContext)
            |false -> Html.none
        
        let currentpage,setpage = React.useState(Types.Page.Builder) 

        React.contextProvider(ModalContext.createModalContext, myModalContext, React.fragment [ //makes the context accesable for the whole project
            Html.div [
                prop.id "contentView"
                prop.children [
                    match currentpage with
                    |Types.Page.Builder -> Builder.Main()
                    |Types.Page.Contact -> Html.div []
                    |Types.Page.Help -> Html.div []
                    modalactivator
                ]
            ]
        ])