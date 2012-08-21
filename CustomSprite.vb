Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry
Imports Rhino.Display
Imports System.Drawing
Imports Grasshopper


Public Class CustomSprite
    Inherits GH_Component
    ''' <summary>
    ''' Each implementation of GH_Component must provide a public 
    ''' constructor without any arguments.
    ''' Category represents the Tab in which the component will appear, 
    ''' Subcategory the panel. If you use non-existing tab or panel names, 
    ''' new tabs/panels will automatically be created.
    ''' </summary>
    Public Sub New()
        MyBase.New("Custom Sprite", "CustomSprite", _
           "Creates a customizable oGL sprite visualization ", _
           "Vector", "Point")
    End Sub

    ''' <summary>
    ''' Registers all the input parameters for this component.
    ''' </summary>
    Protected Overrides Sub RegisterInputParams(ByVal pManager As GH_Component.GH_InputParamManager)
        pManager.Register_StringParam("Sprite", "S", "Sprite file route")
        pManager.Register_ColourParam("Color", "C", "Sprite color", GH_ParamAccess.list)
        pManager.Register_PointParam("Location", "L", "Sprite location", GH_ParamAccess.list)
        pManager.Register_DoubleParam("Size", "S", "Sprite size")
    End Sub

    ''' <summary>
    ''' Registers all the output parameters for this component.
    ''' </summary>
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As GH_Component.GH_OutputParamManager)
    End Sub

    Private customSprite As DisplayBitmap
    Private clusters As List(Of DisplayBitmapDrawList)
    Private size As List(Of Double)
    Private imagePath As String


    Public Overrides Sub DrawViewportWires(ByVal args As Grasshopper.Kernel.IGH_PreviewArgs)
        MyBase.DrawViewportWires(args)

        customSprite = New DisplayBitmap(Image.FromFile(imagePath))

        For i As Integer = 0 To clusters.Count - 1
            args.Display.DrawSprites(customSprite, clusters(i), size(i), True)
        Next
    End Sub


    ''' <summary>
    ''' This is the method that actually does the work.
    ''' </summary>
    ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
    ''' to store data in output parameters.</param>
    ''' 
    Protected Overrides Sub SolveInstance(ByVal DA As IGH_DataAccess)
        Dim ptlist As New List(Of Point3d)
        Dim colors As New List(Of Color)
        Dim dSize As Double


        If (DA.Iteration = 0) Then
            clusters = New List(Of DisplayBitmapDrawList)
            size = New List(Of Double)
        End If

        If (Not DA.GetData(Of String)(0, imagePath)) Then Return
        If (Not DA.GetDataList(Of Point3d)(2, ptlist)) Then Return
        If (Not DA.GetDataList(Of Color)(1, colors)) Then Return

        If (Not DA.GetData(Of Double)(3, dSize)) Then Return

        If colors.Count <> ptlist.Count Then
            If (colors.Count = 1) Then
                Do Until (ptlist.Count = colors.Count)
                    colors.Add(colors.Item(0))
                Loop
            Else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Different number of points and colors supplied")
            End If
        End If

        If ptlist.Count <> 0 Then
            Dim disp As New DisplayBitmapDrawList
            disp.SetPoints(ptlist, colors)
            clusters.Add(disp)
            size.Add(dSize)
        End If

    End Sub

    ''' <summary>
    ''' Provides an Icon for every component that will be visible in the User Interface.
    ''' Icons need to be 24x24 pixels.
    ''' </summary>
    Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
        Get
            'You can add image files to your project resources and access them like this:
            ' return Resources.IconForThisComponent;
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' Each component must have a unique Guid to identify it. 
    ''' It is vital this Guid doesn't change otherwise old ghx files 
    ''' that use the old ID will partially fail during loading.
    ''' </summary>
    Public Overrides ReadOnly Property ComponentGuid() As Guid
        Get
            Return New Guid("{AEBE070F-10C2-44E7-8C9A-D543B90DEEBD}")
        End Get
    End Property
End Class