namespace Components

open Feliz
open Feliz.Bulma
open Types
open Browser
open Browser.Types      

module private Functions =
    let addAnnotation =
        (fun (e:MouseEvent) ->
            let term = window.getSelection().ToString().Trim()
            if term.Length <> 0 then 
                let newAnno = (AnnotationState) //errors are defined in Builder.fs
                newAnno
                |> fun t ->

                t |> setAnnotationState
                t |> setLocalStorageAnnotation "Annotations"
            else 
                ()
            Browser.Dom.window.getSelection().removeAllRanges()
        )

    // let propPlaceHolder = fun e -> () //replace with other functions
open Functions
module Contextmenu =
        
    let private contextmenu (mousex: int, mousey: int) (resetter: MouseEvent-> unit) =
        Html.div [
            prop.onClick addAnnotation
            prop.onClick resetter
        ]

    let initialModal = {
                isActive = false
                location = (0,0)
            }

    let onContextMenu (modalContext:DropdownModal) = 
        let resetter = fun (e:MouseEvent) -> modalContext.setter initialModal //add actual function
        // let rmv = modalContext.setter initialModal 
        contextmenu modalContext.modalState.location resetter



    
            
            

            
            



        


        




        



