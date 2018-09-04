cmake_minimum_required(VERSION 3.0)
set(PLATFORM_SUFFIX 64)

project(boolexpr${PLATFORM_SUFFIX})

set(BOOLEXPR_DIR ${PROJECT_SOURCE_DIR}/../../lib/boolexpr)
set(CMAKE_MODULE_PATH BOOLEXPR_DIR/cmake)
set(VERSION 2.4)
set(BOOST_VERSION 1.54.0)

file(GLOB boolexpr_srcs "${BOOLEXPR_DIR}/include/boolexpr/*.h" "${BOOLEXPR_DIR}/src/*.cc")
file(GLOB_RECURSE glucosamine_srcs "${BOOLEXPR_DIR}/third_party/glucosamine/src/*.h" "${BOOLEXPR_DIR}/third_party/glucosamine/src/core/Solver.cc")
file(GLOB_RECURSE boost_srcs "${BOOLEXPR_DIR}/third_party/boost-${BOOST_VERSION}/*.hpp" "${BOOLEXPR_DIR}/third_party/boost-${BOOST_VERSION}/*.cc")

add_library(boolexpr${PLATFORM_SUFFIX} SHARED ${boolexpr_srcs} ${glucosamine_srcs})
set_property(TARGET boolexpr${PLATFORM_SUFFIX} PROPERTY OUTPUT_NAME boolexpr)

target_compile_options(boolexpr${PLATFORM_SUFFIX} PUBLIC /std:c++11 /Wall)
target_include_directories(boolexpr${PLATFORM_SUFFIX} PUBLIC ${BOOLEXPR_DIR}/include)
target_include_directories(boolexpr${PLATFORM_SUFFIX} PUBLIC ${BOOLEXPR_DIR}/third_party/boost-1.54.0)
target_include_directories(boolexpr${PLATFORM_SUFFIX} PUBLIC ${BOOLEXPR_DIR}/third_party/glucosamine/src)
