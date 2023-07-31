                          NVIDIA Texture Tools 3
                              Version 3.2.3
                  Copyright 2015-2023 NVIDIA Corporation

                      NVIDIA Texture Tools Exporter
                             Version 2023.3.1
                  Copyright 2004-2023 NVIDIA Corporation

The NVIDIA Texture Tools are a set of command-line and GUI tools and an SDK for
compressing textures into various GPU-friendly formats.

To run the Texture Tools Exporter:

   * Run nvtt_export.exe.

For more information of the use of command-line tools:

   * Open a "cmd" window and run each of the executables with the -h argument.

For how to build applications with the NVTT library and its APIs:
  
   * ./docs/index.html

For code samples using the NVTT library, check out https://github.com/nvpro-samples/nvtt_samples.

For comments, feature requests, and bugs, please contact texturetools@nvidia.com.

================================================================================
NVTT Library Features
================================================================================
* Compression/decompression of textures using command-line tools
* Compression/decompression of textures using library APIs
* Supported input formats: DDS, BMP, GIF, HDR, JPG, PGM, PIC, PNG, PPM, PSD, TGA
* Output file formats: DDS, BMP, HDR, JPG, PNG, TGA
* Supported texture formats: 
  - BC1(DXT1)
  - BC2(DXT3) 
  - BC3(DXT5)
  - BC4
  - BC5
  - BC6H
  - BC7
  - ASTC: 2D blocks, low dynamic range 
* CUDA acceleration:
  - Supported for ASTC compression at all quality levels
  - Supported for BCx compression at Default quality, and for BC1 and BC4-7 at
  Fastest and BC1-BC3 at Production and Highest quality
  - Supported for basic image preprocessing

================================================================================
Package Structure 
================================================================================
[PackageRoot]/
             /LICENSE.TXT               ----- license
             /nvbatchcompress.exe       ----- batch compressing tool
             /nvcompress.exe            ----- compressing tool
             /nvddsinfo.exe             ----- tool for showing DDS file information
             /nvdecompress.exe          ----- decompressing tool
             /nvimgdiff.exe             ----- tool for comparing 2 images
In the standalone version:
             /nvtt_export.exe           ----- NVTT Exporter GUI with support for more file formats
In the Photoshop plugin version:
             /nvtt_export.8bi           ----- NVTT Exporter Photoshop plugin
For building programs using NVTT:
             /nvtt*.dll                 ----- redistributable NVTT DLL
             /docs                      ----- API documentation and guide
             /include                   ----- headers
             /lib                       ----- linker library

================================================================================
System Requirements
================================================================================
Operating System: Windows 10 or 11 (64-bit only)

Graphics: OpenGL 3.0+ required for the Texture Tools Exporter

To use the C++ API: MSVC toolset v14x or compiler with a compatible ABI
  (for instance, Visual Studio 2015 through at least 2022 is supported)

To use the C API: Compiler supporting at least C99 and dynamic linking

For CUDA-accelerated compression:
  Any Maxwell+ NVIDIA GPU
  NVIDIA driver 451.48 or newer

================================================================================
About DDS as a Container of Textures
================================================================================
Compressed texture data is stored into DDS files following the DDS specifications:
http://msdn.microsoft.com/en-us/library/bb943991.aspx
https://msdn.microsoft.com/en-us/library/bb173059.aspx

In addition, ASTC formats are defined as follows:

		DXGI_FORMAT_ASTC_4X4_UNORM              = 134,
		DXGI_FORMAT_ASTC_4X4_UNORM_SRGB         = 135,
		DXGI_FORMAT_ASTC_5X4_TYPELESS           = 137,
		DXGI_FORMAT_ASTC_5X4_UNORM              = 138,
		DXGI_FORMAT_ASTC_5X4_UNORM_SRGB         = 139,
		DXGI_FORMAT_ASTC_5X5_TYPELESS           = 141,
		DXGI_FORMAT_ASTC_5X5_UNORM              = 142,
		DXGI_FORMAT_ASTC_5X5_UNORM_SRGB         = 143,
		DXGI_FORMAT_ASTC_6X5_TYPELESS           = 145,
		DXGI_FORMAT_ASTC_6X5_UNORM              = 146,
		DXGI_FORMAT_ASTC_6X5_UNORM_SRGB         = 147,
		DXGI_FORMAT_ASTC_6X6_TYPELESS           = 149,
		DXGI_FORMAT_ASTC_6X6_UNORM              = 150,
		DXGI_FORMAT_ASTC_6X6_UNORM_SRGB         = 151,
		DXGI_FORMAT_ASTC_8X5_TYPELESS           = 153,
		DXGI_FORMAT_ASTC_8X5_UNORM              = 154,
		DXGI_FORMAT_ASTC_8X5_UNORM_SRGB         = 155,
		DXGI_FORMAT_ASTC_8X6_TYPELESS           = 157,
		DXGI_FORMAT_ASTC_8X6_UNORM              = 158,
		DXGI_FORMAT_ASTC_8X6_UNORM_SRGB         = 159,
		DXGI_FORMAT_ASTC_8X8_TYPELESS           = 161,
		DXGI_FORMAT_ASTC_8X8_UNORM              = 162,
		DXGI_FORMAT_ASTC_8X8_UNORM_SRGB         = 163,
		DXGI_FORMAT_ASTC_10X5_TYPELESS          = 165,
		DXGI_FORMAT_ASTC_10X5_UNORM             = 166,
		DXGI_FORMAT_ASTC_10X5_UNORM_SRGB        = 167,
		DXGI_FORMAT_ASTC_10X6_TYPELESS          = 169,
		DXGI_FORMAT_ASTC_10X6_UNORM             = 170,
		DXGI_FORMAT_ASTC_10X6_UNORM_SRGB        = 171,
		DXGI_FORMAT_ASTC_10X8_TYPELESS          = 173,
		DXGI_FORMAT_ASTC_10X8_UNORM             = 174,
		DXGI_FORMAT_ASTC_10X8_UNORM_SRGB        = 175,
		DXGI_FORMAT_ASTC_10X10_TYPELESS         = 177,
		DXGI_FORMAT_ASTC_10X10_UNORM            = 178,
		DXGI_FORMAT_ASTC_10X10_UNORM_SRGB       = 179,
		DXGI_FORMAT_ASTC_12X10_TYPELESS         = 181,
		DXGI_FORMAT_ASTC_12X10_UNORM            = 182,
		DXGI_FORMAT_ASTC_12X10_UNORM_SRGB       = 183,
		DXGI_FORMAT_ASTC_12X12_TYPELESS         = 185,
		DXGI_FORMAT_ASTC_12X12_UNORM            = 186,
		DXGI_FORMAT_ASTC_12X12_UNORM_SRGB       = 187,

================================================================================
Known Issues
================================================================================
* ASTC HDR modes are not supported.

================================================================================
New in 3.2.3
================================================================================
* BC1-BC3: Fixes a bug in 3.2.2 where CUDA compression on Turing GPUs would
produce a `cudaErrorIllegalAddress` error.
* Fixes a bug where `nvtt::Surface::createSubImage` and `nvtt::Surface::diff()`
did not copy the texture type, alpha mode, wrap mode, or normal flag from their
input(s) to their output `nvtt::Surface`.
* Exporter: If an image causes the Exporter to switch from GPU to CPU
compression, the intermediate results are now fully cleared before restarting
image processing. This fixes a bug where images loaded on devices with compute
capability < 5.0 would display with the wrong color space (which would resolve
if the settings were modified).
* Exporter GUI: Fixed bugs in extension autocompletion (for instance, saving a
PNG image named "test" adds the .png extension if test.png does not exist).

================================================================================
New in 3.2.2
================================================================================
* BC1-BC3,BC1a: Improved compression quality at Quality::Production|Highest.
* BC7,ASTC: Fixes a bug where images marked opaque would use a transparent
compressor, and images marked transparent would use an opaque compressor.
* Improved nvcompress' performance on images over 6.25 megapixels.
* Added --max-mip-count, --nim-mip-size, and --no-mip-gamma-correct options to
nvcompress.
* Renamed nvbatchcompress.exe to nvbatchcompress.bat; nvbatchcompress'
functionality has been merged into nvcompress.
* Retargeted CUDA dependency to version 11.8.
* Exporter: Added the Edge Pad (Solidify) effect, which smoothly fills
transparent areas of images.
* Exporter: Added read and write support for the RXGB BC3 normal map format.
* Exporter: Improved power usage by reducing window draw frequency when idle.
* Exporter: File paths are now stored using Unicode instead of ANSI characters.
This fixes a bug where paths with non-ANSI characters would fail to open and
print mojibake error messages.
* Exporter: The DDS reader can now successfully read some images where
DDS_MIPMAPCOUNT is missing from dwFlags.
* Exporter: Improved DDS reader support for uncommon modes such as D3DFMT_CxV8U8
and DDPF_BUMPDUDV.
* Exporter: Fixed a crash when reading bitmasked DDS files with dwRGBBitCount
set to 0 or with subresources between 1 and 7 bytes long.
* Exporter: Fixed a bug where image processing would fail or stall on GPUs with
compute capability less than 5.0.
* Exporter: Normal textures now have the DDPF_NORMAL flag set.
* Exporter: Volume textures now have the DDSCAPS2_VOLUME flag set.
* Exporter: The NVIDIA DDS Read Properties window can now be closed.
* Exporter: Invalid --serialized-effects-v1 strings no longer cause crashes.
* Exporter: Fixed a bug where --mip-filter-width would always be written to
settings, even when Override Filter Width was unchecked.
* Exporter standalone: Fixed a bug where atlas and cubemap settings would be
ignored when reading a command line or preset.
* Exporter standalone: Fixed a race condition where if several dozen nvtt_export
instances were started at the same time, they could mistake each other for the
installer and refuse to launch.

================================================================================
New in 3.2.1
================================================================================
* Optimized CPU-to-GPU transfers, giving faster compression when using the
`nvtt::Context` API and `nvtt::Surface` data is not in VRAM.
* BC1-BC5: Optimized kernel parameters for faster compression.
* BC1/BC1a: Improved compression quality when the input block has close to a
flat color.
* CPU BC1: Fixes a bug where the compressor would sometimes encode transparent
single-color blocks with transparency.
* GPU BC1a: New fast-mode compressor when input data is on the GPU.
* GPU BC6: Improved compressor speed by merging partition kernels.
* Adds `nvtt::Surface::loadFromMemory()`,
`nvtt::SurfaceSet::loadDDSFromMemory()`, and
`nvtt::CubeSurface::loadFromMemory()`. These variants of
`nvtt::Surface::load()`, `nvtt::SurfaceSet::loadDDS()`, and
`nvtt::CubeSurface::load()` work on in-memory data. (Thank you to mijalko on the
NVIDIA Developer Forums.)
* Fixes a bug where the CPU BC1 compressor would sometimes encode transparent
single-color blocks with transparency. (Thank you to tgrimmer on the NVIDIA
Developer Forums.)
* Adds range checks on the parameters of `nvtt::Surface::toRGBE()`,
`nvtt::Surface::fromRGBE()`, and `nvtt::Surface()::quantize()`.
* Updated stb_image dependency.
* Exporter: Fixes a bug where DDS cube maps were written with an arraySize 6x
the correct number.
* Exporter: Improves OpenGL compatibility by removing usage of
`glGetTextureLevelParameteriv()`.
* Exporter: Fixes a bug where `nvtt_export --help` opened the GUI after printing
help info.
* Exporter plugin: Fixes a bug where grayscale images with alpha were
interpreted in (red, alpha, 0, 1) format.

================================================================================
New in 3.2.0
================================================================================
* Adds precompiled kernels for Hopper and Ada GPUs.
* Adds `nvtt::nvtt_encode()` and `nvtt::EncodeSettings` to the low-level API.
`nvtt_encode()` unifies all low-level compression functions under a single
interface, and `nvtt::EncodeSettings` can be extended to add new features
without breaking the API.
* Adds 16-bit half float (`nvtt::ValueType::FLOAT16`) as an input to the
low-level API.
* Adds `nvtt::SetMessageCallback()`. NVTT now reports errors, warnings, and
messages through each thread's message callback. In particular, this can be used
to detect errors even when a function does not return `bool`.
* Adds `nvtt::Surface::gpuDataMutable()`. This allows NVTT Surface data to be
used with your own custom CUDA kernels, without requiring a `const_cast`.
* Adds unclamped sRGB transfer functions. These allow HDR images to be converted
to sRGB and back without significant information loss:
  - `nvtt::Surface::toSrgbUnclamped()`
  - `nvtt::Surface::toLinearFromSrgbUnclamped()`
* Adds `nvtt::Surface::toLinearFromXenonSrgb()`, the inverse of
`nvtt::Surface::toXenonSrgb()`.
* Adds BC3n GPU compressor.
* Faster BC6H GPU compression.
* Fixes an out-of-bounds write within the BC2 GPU compressor.
* Fixes a bug where the C wrapper and C++ Error enumerations didn't match.
* Fixes a bug where the slow-mode BC1 CPU compressor would sometimes output
blocks with alpha, if the input had variegated blocks with semitransparent
pixels. (Thank you to tgrimmer on the NVIDIA Developer Forums.)
* Fixes a bug where `nvtt::Surface::toSrgb()` turned 1.0 into 0.999999940
instead of 1.
* Fixes a bug where `nvttContextQuantize()` was missing from the C wrapper
symbols. (Thank you to mijalko on the NVIDIA Developer Forums.)

================================================================================
New in 3.1.x
================================================================================
3.1.6:
* Added the new API nvtt::useCurrentDevice()
* Moved to CUDA 11
* Added sm_80 and sm_86 (Ampere); move minimum SM from sm_30 to sm_35
* Fixed a memory leak in SurfaceSet
* Added stb_image and stb_image_write as readers and writers
3.1.5:
* Fixed an issue with compression on Quadro K2000 devices
3.1.4:
* Moved to VC 2017
3.1.3:
* Faster BC3 CPU encoding
* Fixed GPU/CPU BC3 discrepancy
3.1.2:
* Stability fixes for CPU encoding
3.1.1:
* Added Surface::demultiplyAlpha()
* Added sm_70 (Volta) and sm_75 (Turing) support
3.1.0:
* Reworked ASTC/BC7 for better performance
* New APIs (nvtt_lowlevel.h) for more straightforward texture compression
* Better quality for BC6 - fast mode.

================================================================================
Revision History
================================================================================
2023/7/5    NVIDIA Texture Tools 3.2.2
2023/3/28   NVIDIA Texture Tools 3.2.1
2023/2/23   NVIDIA Texture Tools 3.2.0
2021/11/8   NVIDIA Texture Tools 3.1.6
2020/8/3    NVIDIA Texture Tools 3.1.5
2020/6/13   NVIDIA Texture Tools 3.1.4
2020/2/12   NVIDIA Texture Tools 3.1.3
2020/2/4    NVIDIA Texture Tools 3.1.2
2019/7/8    NVIDIA Texture Tools 3.1.0
2019/2/13   NVIDIA Texture Tools 3.0.2
2018/11/1   NVIDIA Texture Tools 3.0.1
2018/9/20   NVIDIA Texture Tools 3 Beta 9.7
2018/5/30   NVIDIA Texture Tools 3 Beta 9.6
2018/5/15   NVIDIA Texture Tools 3 Beta 9.5
2018/4/23   NVIDIA Texture Tools 3 Beta 9.4
2018/1/19   NVIDIA Texture Tools 3 Beta 9.3
2018/1/12   NVIDIA Texture Tools 3 Beta 9.2
2017/12/19  NVIDIA Texture Tools 3 Beta 9.1
2017/11/1   NVIDIA Texture Tools 3 Beta 9
2017/4/6    NVIDIA Texture Tools 3 Beta 8.4
2017/3/21   NVIDIA Texture Tools 3 Beta 8.3
2017/3/15   NVIDIA Texture Tools 3 Beta 8.2
2017/2/28   NVIDIA Texture Tools 3 Beta 8.1
2017/2/15   NVIDIA Texture Tools 3 Beta 8
2017/2/9    NVIDIA Texture Tools 3 Beta 7.1
2017/2/8    NVIDIA Texture Tools 3 Beta 7
2017/1/18   NVIDIA Texture Tools 3 Beta 6
2016/11/22  NVIDIA Texture Tools 3 Beta 5.2
2016/10/24  NVIDIA Texture Tools 3 Beta 5.1
2016/10/19  NVIDIA Texture Tools 3 Beta 5
2016/9/7    NVIDIA Texture Tools 3 Beta 4
2016/7/8    NVIDIA Texture Tools 3 Beta 3
2016/7/1    NVIDIA Texture Tools 3 Beta 2
2016/3/23   NVIDIA Texture Tools 3 Beta 1
2016/2/5    NVIDIA Texture Tools 3 Alpha 3
2015/11/18  NVIDIA Texture Tools 3 Alpha 2
2015/10/28  NVIDIA Texture Tools 3 Alpha 1

================================================================================
Credits
================================================================================

NVIDIA Texture Tools by Fei Yang, Ignacio Castaño, Neil Bickford,
Roberto Teixeira, Tadahito Kobayashi, and contributors.

NVIDIA Texture Tools Exporter by Neil Bickford and Chris Hebert. User interface
designed by Boris Ustaev.

Original Exporter plugins by Eric Foo and Doug Rogers, with contributions from
Fabio Policarpo.

Thanks to Alexandre Sambo, Alexey Panteleev, Andres Garcia, Andrew Page,
Anitha Pai, Ashlee Martino-Tarr, Brian Burke, Christophe Kulesza,
Christophe Soum, Chu Tang, David Akeley, Doug Mendez, Erika Dignam, Fei Yang,
Florent Rouat, Frederick Patton, Guilin Liu, Guillaume Polaillon,
Jeetendra Malkani, Jhazmin Ledezma-Bacuetes, Justin Thomas, Lorena Perez,
Luka Mijalkovic, Mark Henderson, Michael Songy, Michael Steele, Oxicid,
Riley Alston, Rick Grandy, Samarpit Masih, Sean Kilbride, Sheetal Jangavali,
Stanley Tack, Susan Fintz, Tristan Grimmer, Tristan Lorach,
and Vincent Brisebois.

And to you, for reading these credits.
