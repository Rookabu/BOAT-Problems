namespace App

open Feliz
open Feliz.Bulma
open Types
open Components

type View =
    [<ReactComponent>]
    static member Main() =

        let (modalState: ModalInfo, setModal) =
            React.useState(Contextmenu.initialModal)               
                       
        let myModalContext = { //makes setter and state in one record type
            modalState = modalState
            setter = setModal
            }

        let modalactivator = 
            match modalState.isActive with
                |true -> Contextmenu.onContextMenu (myModalContext)
                |false -> Html.none
        
        let currentpage,setpage = React.useState(Types.Page.Builder) 

        printfn "%A" currentpage
        React.contextProvider(Contexts.ModalContext.createModalContext, myModalContext, React.fragment [ //makes the context accesable for the whole project
            Html.div [
                prop.id "contentView"
                prop.children [
                    match currentpage with
                    |Types.Page.Builder -> Components.Builder.Main()
                    |Types.Page.Contact -> Html.div []
                    |Types.Page.Help -> Html.div []
                    modalactivator
                ]
            ]
        ])